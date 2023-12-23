using Microsoft.Extensions.Hosting;
using Teanuts;
using Teanuts.Host;

var host = ApplicationHost.Build<App, MainWindow>(args, hostBuilder =>
{

});

host.Run();
