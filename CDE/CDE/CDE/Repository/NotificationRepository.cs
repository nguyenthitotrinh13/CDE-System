//using CDE.DBContexts;
//using CDE.Models;

//namespace CDE.Repository
//{
//    public class NotificationRepository
//    {
//        private readonly ApplicationDbContext _context;

//        public NotificationRepository(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        // Gửi thông báo cho người dùng
//        public async Task SendNotificationAsync(Guid userId, string message)
//        {
//            var notification = new Notification
//            {
//                UserId = userId,
//                Message = message,
//                CreatedAt = DateTime.UtcNow
//            };

//            _context.Notifications.Add(notification);
//            await _context.SaveChangesAsync();
//        }

//        // Lấy tất cả thông báo của người dùng
//        public async Task<IEnumerable<Notification>> GetNotificationsForUserAsync(Guid userId)
//        {
//            return await _context.Notifications
//                .Where(n => n.UserId == userId)
//                .OrderByDescending(n => n.CreatedAt)
//                .ToListAsync();
//        }

//        // Đánh dấu thông báo là đã đọc
//        public async Task MarkNotificationAsReadAsync(Guid notificationId)
//        {
//            var notification = await _context.Notifications
//                .FirstOrDefaultAsync(n => n.Id == notificationId);

//            if (notification != null)
//            {
//                notification.IsRead = true;
//                await _context.SaveChangesAsync();
//            }
//        }
//    }
//}
