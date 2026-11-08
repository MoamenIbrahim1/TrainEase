using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TrainEase.Core.Application.Abstrations.Model.Delays;
using TrainEase.Core.Application.Abstrations.Model.Stations;
using TrainEase.Core.Application.Abstrations.Services;
using TrainEase.Core.Application.Exceptions;
using TrainEase.Core.Domain.Contracts;
using TrainEase.Core.Domain.Entities;
using TrainEase.Core.Domain.Entities.Enums;

namespace TrainEase.Core.Application.Services.Delays
{
    public class TrainDelayService(
        IUnitOfWork unitOfWork,
        IEmailService emailService,
        UserManager<User> userManager) : ITrainDelayService
    {
        
        public async Task<TrainDelayResultDto> ReportDelayAsync(
            ReportDelayDto dto, string stationMasterUserId)
        {
            var stationMasterUser = await userManager
                .Users
                .Include(u => u.station)
                .FirstOrDefaultAsync(u => u.Id == stationMasterUserId);

            if (stationMasterUser is null || stationMasterUser.Role != UserRole.StationMaster)
                throw new UnAuthorizedException("Only station masters can report delays.");

            if (stationMasterUser.StationId is null)
                throw new BadRequestException("Station master is not assigned to a station.");

            if (dto.DelayMinutes.HasValue && dto.DelayMinutes.Value <= 0)
                throw new BadRequestException("Delay minutes must be greater than zero.");

            var scheduleRepo = unitOfWork.GetRepository<Schedule, int>();
            var schedule = await scheduleRepo
                .GetQueryable(s => s.Id == dto.ScheduleId && !s.IsDeleted)
                .Include(s => s.Train)
                .Include(s => s.SourceStation)
                .Include(s => s.DestinationStation)
                .FirstOrDefaultAsync();

            if (schedule is null)
                throw new NotFoundException("Schedule", dto.ScheduleId);

            if (schedule.SourceStationId != stationMasterUser.StationId)
                throw new UnAuthorizedException("You can only report delays for schedules in your station.");

            var bookingRepo = unitOfWork.GetRepository<TrainEase.Core.Domain.Entities.Booking, int>();
            var confirmedBookings = await bookingRepo
                .GetQueryable(b =>
                    b.ScheduleId == dto.ScheduleId &&
                    b.Status == BookingStatus.Confirmed &&
                    !b.IsDeleted)
                .Include(b => b.User)
                .Include(b => b.Tickets)
                    .ThenInclude(t => t.Passenger)
                .ToListAsync();

            var originalTime = schedule.DepartureTime.ToString("hh:mm tt");

            var newTime = dto.DelayMinutes.HasValue
                ? schedule.DepartureTime.AddMinutes(dto.DelayMinutes.Value).ToString("hh:mm tt")
                : null;

            var notificationMessage = BuildNotificationMessage(
                trainNumber: schedule.Train.TrainNumber!,
                delayMinutes: dto.DelayMinutes,
                newTime: newTime
            );

            var notifiedEmails = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var notifiedBookingOwners = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var notificationRepo = unitOfWork.GetRepository<Notification, int>();
            var emailTasks = new List<Task>();
            var notifiedCount = 0;

            foreach (var booking in confirmedBookings)
            {
                var emailRecipients = new List<(string Email, string Name)>();

                // 1) صاحب الحجز
                if (IsRealEmail(booking.User?.Email))
                    emailRecipients.Add((booking.User!.Email!, booking.User.FullName ?? "عزيزنا الراكب"));

                // 2) المرافقين
                foreach (var ticket in booking.Tickets)
                {
                    var passenger = ticket.Passenger;
                    if (passenger is not null && IsRealEmail(passenger.Email))
                        emailRecipients.Add((passenger.Email!, passenger.Name ?? "عزيزنا الراكب"));
                }

                foreach (var recipient in emailRecipients)
                {
                    if (!notifiedEmails.Add(recipient.Email))
                        continue;

                    var emailBody = BuildDelayEmailBody(
                        userName: recipient.Name,
                        trainNumber: schedule.Train.TrainNumber!,
                        trainName: schedule.Train.TrainName!,
                        from: schedule.SourceStation!.Name!,
                        to: schedule.DestinationStation!.Name!,
                        delayMinutes: dto.DelayMinutes,
                        originalTime: originalTime,
                        newTime: newTime,
                        stationName: stationMasterUser.station!.Name!
                    );

                    emailTasks.Add(emailService.SendEmailAsync(
                        to: recipient.Email,
                        subject: $"⚠️ تأخير في رحلتك — قطار {schedule.Train.TrainNumber}",
                        body: emailBody
                    ));

                    notifiedCount++;
                }

                // Notification داخلي لصاحب الحجز فقط
                if (!string.IsNullOrWhiteSpace(booking.UserId) &&
                    notifiedBookingOwners.Add(booking.UserId))
                {
                    await notificationRepo.AddAsync(new Notification
                    {
                        UserId = booking.UserId,
                        SentByUserId = stationMasterUserId,
                        Message = notificationMessage,
                        SentAt = DateTime.UtcNow,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            await Task.WhenAll(emailTasks);

            var delayRepo = unitOfWork.GetRepository<TrainDelay, int>();
            var delay = new TrainDelay
            {
                ScheduleId = dto.ScheduleId,
                ReportedByUserId = stationMasterUserId,
                DelayMinutes = dto.DelayMinutes,
                ReportedAt = DateTime.UtcNow,
                NotifiedPassengersCount = notifiedCount,
                CreatedAt = DateTime.UtcNow
            };

            await delayRepo.AddAsync(delay);
            await unitOfWork.CompeleteAsync();

            return new TrainDelayResultDto
            {
                DelayId = delay.Id,
                ScheduleId = delay.ScheduleId,
                TrainNumber = schedule.Train.TrainNumber!,
                TrainName = schedule.Train.TrainName!,
                DelayMinutes = dto.DelayMinutes,
                ReportedAt = delay.ReportedAt,
                NotifiedPassengersCount = notifiedCount,
                StationName = stationMasterUser.station!.Name!
            };
        }

      
        public async Task<IEnumerable<TrainDelayResultDto>> GetAllDelaysAsync()
        {
            var delayRepo = unitOfWork.GetRepository<TrainDelay, int>();

            var delays = await delayRepo
                .GetQueryable(d => !d.IsDeleted)
                .Include(d => d.Schedule).ThenInclude(s => s.Train)
                .Include(d => d.Schedule).ThenInclude(s => s.SourceStation)
                .Include(d => d.Schedule).ThenInclude(s => s.DestinationStation)
                .Include(d => d.ReportedByUser).ThenInclude(u => u.station)
                .OrderByDescending(d => d.ReportedAt)
                .ToListAsync();

            return delays.Select(MapToDto).ToList();
        }

        public async Task<IEnumerable<TrainDelayResultDto>> GetDelaysByStationAsync(int stationId)
        {
            var delayRepo = unitOfWork.GetRepository<TrainDelay, int>();

            var delays = await delayRepo
                .GetQueryable(d => !d.IsDeleted && d.ReportedByUser.StationId == stationId)
                .Include(d => d.Schedule).ThenInclude(s => s.Train)
                .Include(d => d.Schedule).ThenInclude(s => s.SourceStation)
                .Include(d => d.Schedule).ThenInclude(s => s.DestinationStation)
                .Include(d => d.ReportedByUser).ThenInclude(u => u.station)
                .OrderByDescending(d => d.ReportedAt)
                .ToListAsync();

            return delays.Select(MapToDto).ToList();
        }

      
        public async Task<int> GetStationIdByUserAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user?.StationId is null)
                throw new BadRequestException("Station master is not assigned to a station.");
            return user.StationId.Value;
        }

        
        public async Task<IEnumerable<StationScheduleDto>> GetSchedulesByStationAsync(int stationId)
        {
            var scheduleRepo = unitOfWork.GetRepository<Schedule, int>();

            var schedules = await scheduleRepo
                .GetQueryable(s =>
                    s.SourceStationId == stationId &&
                    !s.IsDeleted &&
                    s.DepartureTime >= DateTime.UtcNow)
                .Include(s => s.Train)
                .Include(s => s.DestinationStation)
                .OrderBy(s => s.DepartureTime)
                .ToListAsync();

            return schedules.Select(s => new StationScheduleDto
            {
                ScheduleId = s.Id,
                TrainNumber = s.Train.TrainNumber!,
                TrainName = s.Train.TrainName!,
                DestinationStation = s.DestinationStation!.Name!,
                DepartureTime = s.DepartureTime
            }).ToList();
        }

        private static string BuildNotificationMessage(
            string trainNumber,
            int? delayMinutes,
            string? newTime)
        {
            // الرسالة الأساسية
            var msg = delayMinutes.HasValue && newTime is not null
                ? $"تأخير {delayMinutes} دقيقة في قطار {trainNumber} — الموعد الجديد: {newTime}."
                : $"قطار {trainNumber} سيتأخر عن موعده.";

          
            return msg;
        }

        
        private static bool IsRealEmail(string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            var normalized = email.Trim().ToLower();

            if (normalized.EndsWith("@trainease.com"))
                return false;

            return normalized.Contains("@");
        }

        
        private static TrainDelayResultDto MapToDto(TrainDelay d) => new()
        {
            DelayId = d.Id,
            ScheduleId = d.ScheduleId,
            TrainNumber = d.Schedule.Train.TrainNumber!,
            TrainName = d.Schedule.Train.TrainName!,
            DelayMinutes = d.DelayMinutes,
            ReportedAt = d.ReportedAt,
            NotifiedPassengersCount = d.NotifiedPassengersCount,
            StationName =
                 d.ReportedByUser?.station?.Name
                 ?? d.Schedule?.SourceStation?.Name
                 ?? "Unknown Station"        
        };

      
        private static string BuildDelayEmailBody(
            string userName,
            string trainNumber,
            string trainName,
            string from,
            string to,
            int? delayMinutes,
            string originalTime,
            string? newTime,
            string stationName)
        {
            // ✅ صفوف الوقت — تظهر بس لو delayMinutes موجودة
            var timeRows = delayMinutes.HasValue && newTime is not null
                ? $"""
                  <tr>
                    <td style="padding:8px 0;color:#555;">الموعد الأصلي</td>
                    <td style="padding:8px 0;font-weight:bold;
                               text-decoration:line-through;color:#e74c3c;">
                      {originalTime}
                    </td>
                  </tr>
                  <tr>
                    <td style="padding:8px 0;color:#555;">الموعد الجديد</td>
                    <td style="padding:8px 0;font-weight:bold;color:#27ae60;font-size:18px;">
                      {newTime}
                    </td>
                  </tr>
                  <tr>
                    <td style="padding:8px 0;color:#555;">مدة التأخير</td>
                    <td style="padding:8px 0;font-weight:bold;color:#e67e22;">
                      +{delayMinutes} دقيقة
                    </td>
                  </tr>
                  """
                : "";


            // ✅ نص الـ note يتغير حسب المعلومات المتاحة
            var noteText = delayMinutes.HasValue
                ? $"يرجى التوجه للمحطة في الموعد الجديد ({newTime}). نعتذر عن أي إزعاج."
                : "يرجى متابعة لوحات المحطة للاطلاع على الموعد الجديد. نعتذر عن أي إزعاج.";

            return $"""
                <!DOCTYPE html>
                <html dir="rtl" lang="ar">
                <head><meta charset="UTF-8"/></head>
                <body style="margin:0;padding:0;background:#f4f6f8;font-family:Arial,sans-serif;">
                  <table width="100%" cellpadding="0" cellspacing="0"
                         style="background:#f4f6f8;padding:40px 0;">
                    <tr><td align="center">
                      <table width="600" cellpadding="0" cellspacing="0"
                             style="background:#fff;border-radius:12px;overflow:hidden;
                                    box-shadow:0 2px 8px rgba(0,0,0,0.08);">

                        <!-- Header -->
                        <tr>
                          <td style="background:#e67e22;padding:32px;text-align:center;">
                            <h1 style="margin:0;color:#fff;font-size:24px;">🚆 TrainEase</h1>
                            <p style="margin:8px 0 0;color:#fdebd0;font-size:14px;">تنبيه تأخير رحلة</p>
                          </td>
                        </tr>

                        <!-- Body -->
                        <tr>
                          <td style="padding:36px 40px;">
                            <h2 style="color:#e67e22;margin:0 0 8px;">⚠️ تأخير في رحلتك</h2>
                            <p style="color:#555;font-size:15px;margin:0 0 24px;">
                              مرحباً <strong>{userName}</strong>،<br/>
                              نود إعلامك بأن رحلتك ستتأخر عن موعدها المحدد.
                            </p>

                            <!-- Details -->
                            <table width="100%" cellpadding="0" cellspacing="0"
                                   style="background:#fef9f0;border-radius:8px;
                                          padding:20px 24px;border:1px solid #f0c080;">
                              <tr>
                                <td style="padding:8px 0;color:#555;">القطار</td>
                                <td style="padding:8px 0;font-weight:bold;color:#222;">
                                  {trainName} (#{trainNumber})
                                </td>
                              </tr>
                              <tr>
                                <td style="padding:8px 0;color:#555;">من</td>
                                <td style="padding:8px 0;font-weight:bold;color:#222;">{from}</td>
                              </tr>
                              <tr>
                                <td style="padding:8px 0;color:#555;">إلى</td>
                                <td style="padding:8px 0;font-weight:bold;color:#222;">{to}</td>
                              </tr>
                              {timeRows}
                              <tr>
                                <td style="padding:8px 0;color:#555;">المحطة المُبلِّغة</td>
                                <td style="padding:8px 0;font-weight:bold;color:#222;">
                                  {stationName}
                                </td>
                              </tr>
                              <tr>
                                <td style="padding:8px 0;color:#555;">وقت الإبلاغ</td>
                                <td style="padding:8px 0;font-weight:bold;color:#222;">
                                  {DateTime.UtcNow:dd/MM/yyyy hh:mm tt}
                                </td>
                              </tr>
                            </table>

                            <!-- Note -->
                            <p style="margin:24px 0 0;padding:16px;background:#fef3cd;
                                       border-radius:8px;border-right:4px solid #e67e22;
                                       color:#856404;font-size:14px;">
                              💡 {noteText}
                            </p>
                          </td>
                        </tr>

                        <!-- Footer -->
                        <tr>
                          <td style="background:#f8f9fa;padding:20px 40px;
                                     text-align:center;border-top:1px solid #dee2e6;">
                            <p style="margin:0;color:#aaa;font-size:12px;">
                              © {DateTime.UtcNow.Year} TrainEase — جميع الحقوق محفوظة
                            </p>
                          </td>
                        </tr>

                      </table>
                    </td></tr>
                  </table>
                </body>
                </html>
                """;
        }
    }
}