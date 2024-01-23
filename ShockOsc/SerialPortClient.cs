using OpenShock.ShockOsc.Models;
using Serilog;
using System.Diagnostics;
using System.IO.Ports;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using ILogger = Serilog.ILogger;

namespace OpenShock.ShockOsc;

public class rftransmit
{
    public required string model { get; set; }
    public required int id { get; set; }
    public string type { get; set; }
    public int intensity { get; set; }
    public int durationMs { get; set; }
}

public static class SerialPortClient
{
    private static readonly ILogger Logger = Log.ForContext(typeof(SerialPortClient));
    private static SerialPort SerialPort;
    private static Channel<string> TxChannel = Channel.CreateUnbounded<string>(new UnboundedChannelOptions());

    static SerialPortClient()
    {
        SerialPort = new SerialPort
        {
            PortName = Config.ConfigInstance.SerialPort.PortName,
            BaudRate = Config.ConfigInstance.SerialPort.BaudRate ?? 115200,
            Parity = Parity.None,
            DataBits = 8,
            StopBits = StopBits.One,
            Handshake = Handshake.None,
            ReadTimeout = 500,
            WriteTimeout = 500,
            NewLine = "\r\n"
        };
    }

    public static void Open()
    {
        Logger.Information("Opening serial port {0}", Config.ConfigInstance.SerialPort.PortName);
        SerialPort.Open();
        Task.Run(() => TxPumpAsync());
    }

    private static async Task TxPumpAsync()
    {
        try
        {
            while (SerialPort.IsOpen)
            {
                string sCmd = await TxChannel.Reader.ReadAsync();
                byte[] buffer = Encoding.ASCII.GetBytes($"{sCmd}\r\n");
                await SerialPort.BaseStream.WriteAsync(buffer, 0, buffer.Length);// sCmd.Length + 2);
                await SerialPort.BaseStream.FlushAsync();
                Logger.Information($"Sent command {sCmd}");
            }
        }
        catch (Exception)
        {
            Debug.WriteLine($"{DateTime.Now.ToString("HH:mm:ss:fff")} TxCommandsAsync: Exception");
        }
    }

    public static Task Control(params Control[] data)
    {
        return Task.WhenAll(data.Where(control => control.rftransmit != null).Select(control =>
        {
            rftransmit rftransmit = control.rftransmit;
            rftransmit.type = control.Type == ControlType.Shock ? "shock" : control.Type == ControlType.Vibrate ? "vibrate" : control.Type == ControlType.Sound ?
                    "sound" : control.Type == ControlType.Stop ? "stop" : "";
            rftransmit.intensity = control.Intensity;
            rftransmit.durationMs = (int)control.Duration;
            string command = String.Format("{0} {1}", "rftransmit", JsonSerializer.Serialize(rftransmit));
            Logger.Information("Sending line: {0}", command);
            return TxChannel.Writer.WriteAsync(command).AsTask();
        }).ToArray());
    }
}
