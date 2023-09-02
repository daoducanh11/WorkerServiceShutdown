using System.Diagnostics;

namespace WorkerServiceShutdown
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private IConfiguration _configuration;

        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            int hour = int.Parse(_configuration["Shutdown:Hour"]);
            int delay = int.Parse(_configuration["Shutdown:Delay"]);

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                if(hour <= DateTime.Now.Hour)
                {
                    // Tạo một đối tượng ProcessStartInfo
                    ProcessStartInfo psi = new ProcessStartInfo();
                    // Thiết lập tên file là shutdown
                    psi.FileName = "shutdown";
                    // Thiết lập các tham số cho lệnh shutdown
                    // /s: tắt máy tính
                    // /f: buộc đóng các ứng dụng đang chạy
                    // /t 0: không đếm ngược thời gian
                    psi.Arguments = "/s /f /t 0";
                    // Khởi chạy quá trình
                    Process.Start(psi);
                }

                await Task.Delay(1000 * 60 * delay, stoppingToken);
            }
        }
    }
}