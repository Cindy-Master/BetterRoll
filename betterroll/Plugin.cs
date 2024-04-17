using Dalamud.Interface.Windowing;
using Dalamud.Plugin;

namespace betterroll;

public sealed class Plugin : IDalamudPlugin
{
    public DalamudPluginInterface Dalamud { get; init; }

    public WindowSystem WindowSystem = new("betterroll");
    private MainWindow _wnd;

    public Plugin(DalamudPluginInterface dalamud)
    {
        dalamud.Create<Service>();

        _wnd = new MainWindow(dalamud);
        WindowSystem.AddWindow(_wnd);

        Dalamud = dalamud;
        dalamud.UiBuilder.DisableAutomaticUiHide = true;
        Dalamud.UiBuilder.Draw += WindowSystem.Draw;
        //Dalamud.UiBuilder.OpenConfigUi += () => _wndConfig.IsOpen = true;
    }

    public void Dispose()
    {
        WindowSystem.RemoveAllWindows();
        _wnd.Dispose();
    }
}
