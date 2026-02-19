using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Capabilities;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Utils;
using CS2_GameHUDAPI;
using Microsoft.Extensions.Logging;
using System.Drawing;
using System.Globalization;
using System.Text.Json.Serialization;
using Timer = CounterStrikeSharp.API.Modules.Timers.Timer;

namespace CS2_C4TimerHUD;

public class CS2_C4TimerConfig : BasePluginConfig
{
    /// <summary>
    /// Включить/выключить плагин (по умолчанию: 1)
    /// 0 - выключен, 1 - включен
    /// </summary>
    [JsonPropertyName("css_c4timer_enabled")]
    public int Enabled { get; set; } = 1;

    /// <summary>
    /// Использовать динамический цвет таймера (по умолчанию: 1)
    /// 0 - использовать фиксированный цвет из Color
    /// 1 - использовать цвета из TimeColor
    /// </summary>
    [JsonPropertyName("css_c4timer_use_dynamic_color")]
    public int UseDynamicColor { get; set; } = 1;

    /// <summary>
    /// Фиксированный цвет текста (если динамический цвет выключен) (по умолчанию: "255 255 255")
    /// Поддерживаются форматы: RGB ("255 0 0"), HEX ("#FF0000"), название ("red")
    /// </summary>
    [JsonPropertyName("css_c4timer_color")]
    public string Color { get; set; } = "255 255 255";

    /// <summary>
    /// Динамические цвета для секунд (по умолчанию: "20:yellow,10:red,5:darkred")
    /// Формат: секунда:цвет,секунда:цвет,... (цвета применяются от 0 до указанной секунды)
    /// </summary>
    [JsonPropertyName("css_c4timer_timecolor")]
    public string TimeColor { get; set; } = "20:yellow,10:red,5:darkred";

    /// <summary>
    /// Канал HUD для отображения (0-255) (по умолчанию: 2)
    /// </summary>
    [JsonPropertyName("css_c4timer_hud_channel")]
    public int HudChannel { get; set; } = 2;

    /// <summary>
    /// Позиция HUD по оси X (по умолчанию: 1.3)
    /// Диапазон: -100.0 до 100.0
    /// </summary>
    [JsonPropertyName("css_c4timer_hud_x")]
    public float HudX { get; set; } = 1.3f;

    /// <summary>
    /// Позиция HUD по оси Y (по умолчанию: -3.9)
    /// Диапазон: -100.0 до 100.0
    /// </summary>
    [JsonPropertyName("css_c4timer_hud_y")]
    public float HudY { get; set; } = -3.9f;

    /// <summary>
    /// Позиция HUD по оси Z (высота) (по умолчанию: 6.7)
    /// Диапазон: 0.0 до 200.0
    /// </summary>
    [JsonPropertyName("css_c4timer_hud_z")]
    public float HudZ { get; set; } = 6.7f;

    /// <summary>
    /// Размер шрифта для HUD (по умолчанию: 100)
    /// Диапазон: 10-200
    /// </summary>
    [JsonPropertyName("css_c4timer_font_size")]
    public int FontSize { get; set; } = 100;

    /// <summary>
    /// Имя шрифта для HUD (по умолчанию: "Consolas")
    /// </summary>
    [JsonPropertyName("css_c4timer_font_name")]
    public string FontName { get; set; } = "Consolas";

    /// <summary>
    /// Единиц на пиксель для шрифта (по умолчанию: 0.0057)
    /// Диапазон: 0.001 до 1.0
    /// </summary>
    [JsonPropertyName("css_c4timer_units_per_px")]
    public float UnitsPerPixel { get; set; } = 0.0057f;

    /// <summary>
    /// Толщина обводки текста (0.0 = нет обводки) (по умолчанию: 0.0)
    /// Диапазон: 0.0 до 10.0
    /// </summary>
    [JsonPropertyName("css_c4timer_text_border_width")]
    public float TextBorderWidth { get; set; } = 0.0f;

    /// <summary>
    /// Высота обводки текста (0.0 = нет обводки) (по умолчанию: 0.0)
    /// Диапазон: 0.0 до 10.0
    /// </summary>
    [JsonPropertyName("css_c4timer_text_border_height")]
    public float TextBorderHeight { get; set; } = 0.0f;

    /// <summary>
    /// Использовать жирный шрифт (по умолчанию: 1)
    /// 0 - обычный, 1 - жирный
    /// </summary>
    [JsonPropertyName("css_c4timer_use_bold_font")]
    public int UseBoldFont { get; set; } = 1;

    /// <summary>
    /// Метод отображения HUD (по умолчанию: 1)
    /// 0 - телепорт (быстрый, может трястись)
    /// 1 - point_orient (стабильный)
    /// </summary>
    [JsonPropertyName("css_c4timer_hud_method")]
    public int HudMethod { get; set; } = 1;

    /// <summary>
    /// Символ для отображения взорванной C4 (по умолчанию: "Ⓧ")
    /// </summary>
    [JsonPropertyName("css_c4timer_exploded_symbol")]
    public string ExplodedSymbol { get; set; } = "Ⓧ";

    /// <summary>
    /// Символ для отображения обезвреженной C4 (по умолчанию: "Ⓥ")
    /// </summary>
    [JsonPropertyName("css_c4timer_defused_symbol")]
    public string DefusedSymbol { get; set; } = "Ⓥ";

    /// <summary>
    /// Уровень логирования: 0-Trace, 1-Debug, 2-Information, 3-Warning, 4-Error, 5-Critical
    /// </summary>
    [JsonPropertyName("css_c4timer_log_level")]
    public int LogLevel { get; set; } = 4;
}

[MinimumApiVersion(362)]
public class CS2_C4Timer : BasePlugin, IPluginConfig<CS2_C4TimerConfig>
{
    public override string ModuleName => "CS2 C4TimerHUD";
    public override string ModuleVersion => "1.9";
    public override string ModuleAuthor => "Fixed by le1t1337 + AI DeepSeek. Code logic by belom0r";

    public required CS2_C4TimerConfig Config { get; set; }

    private static IGameHUDAPI? _hudapi;
    private readonly Dictionary<int, string> _timeColorMap = new();
    private readonly Dictionary<int, bool> _hudInitialized = new();
    private readonly Dictionary<int, Color> _playerCurrentColor = new();

    private bool _c4Planted;
    private bool _c4Exploded;
    private bool _c4Defused;
    private int _timerLength;
    private int _timerCountdown;
    private Timer? _countdownTimer;
    private Color _fixedColor = Color.White;
    private string _lastMessage = "";
    private Color _lastColor = Color.White;

    public void OnConfigParsed(CS2_C4TimerConfig config)
    {
        config.Enabled = Math.Clamp(config.Enabled, 0, 1);
        config.UseDynamicColor = Math.Clamp(config.UseDynamicColor, 0, 1);
        config.HudChannel = Math.Clamp(config.HudChannel, 0, 255);
        config.HudX = Math.Clamp(config.HudX, -100.0f, 100.0f);
        config.HudY = Math.Clamp(config.HudY, -100.0f, 100.0f);
        config.HudZ = Math.Clamp(config.HudZ, 0.0f, 200.0f);
        config.FontSize = Math.Clamp(config.FontSize, 10, 200);
        config.UnitsPerPixel = Math.Clamp(config.UnitsPerPixel, 0.001f, 1.0f);
        config.TextBorderWidth = Math.Clamp(config.TextBorderWidth, 0.0f, 10.0f);
        config.TextBorderHeight = Math.Clamp(config.TextBorderHeight, 0.0f, 10.0f);
        config.UseBoldFont = Math.Clamp(config.UseBoldFont, 0, 1);
        config.HudMethod = Math.Clamp(config.HudMethod, 0, 1);
        config.LogLevel = Math.Clamp(config.LogLevel, 0, 5);

        if (string.IsNullOrEmpty(config.ExplodedSymbol)) config.ExplodedSymbol = "Ⓧ";
        if (string.IsNullOrEmpty(config.DefusedSymbol)) config.DefusedSymbol = "Ⓥ";

        if (!TryParseColor(config.Color, out _fixedColor))
        {
            _fixedColor = Color.White;
            Log(LogLevel.Warning, $"Неверный формат цвета '{config.Color}', используется белый.");
        }

        ParseTimeColorConfig(config.TimeColor);
        Config = config;
    }

    private void ParseTimeColorConfig(string timeColorStr)
    {
        _timeColorMap.Clear();
        for (int i = 0; i <= 60; i++)
            _timeColorMap[i] = "white";

        if (string.IsNullOrEmpty(timeColorStr)) return;

        foreach (var part in timeColorStr.Split(',', StringSplitOptions.RemoveEmptyEntries))
        {
            var elements = part.Split(':', StringSplitOptions.RemoveEmptyEntries);
            if (elements.Length != 2) continue;

            if (int.TryParse(elements[0], out int sec) && sec >= 0 && sec <= 60)
            {
                string colorName = elements[1].Trim().ToLower();
                if (IsKnownColor(colorName))
                {
                    for (int i = 0; i <= sec; i++)
                        _timeColorMap[i] = colorName;
                }
            }
        }
    }

    private bool IsKnownColor(string colorName) => colorName switch
    {
        "white" or "red" or "yellow" or "green" or "blue" or "black" or "darkred" or "orange" or "purple" => true,
        _ => false
    };

    private Color GetColorForSecond(int second)
    {
        if (Config.UseDynamicColor == 1)
        {
            if (_timeColorMap.TryGetValue(second, out string? colorName))
                return ColorFromName(colorName);
            return Color.White;
        }
        return _fixedColor;
    }

    private Color ColorFromName(string name) => name switch
    {
        "white" => Color.White,
        "red" => Color.Red,
        "yellow" => Color.Yellow,
        "green" => Color.Green,
        "blue" => Color.Blue,
        "black" => Color.Black,
        "darkred" => Color.FromArgb(139, 0, 0),
        "orange" => Color.Orange,
        "purple" => Color.Purple,
        _ => Color.White
    };

    private bool TryParseColor(string input, out Color color)
    {
        color = Color.White;
        if (string.IsNullOrWhiteSpace(input)) return false;
        input = input.Trim();

        if (input.StartsWith("#"))
        {
            try { color = ColorTranslator.FromHtml(input); return true; } catch { }
        }

        try
        {
            color = Color.FromName(input);
            if (color.IsKnownColor) return true;
        }
        catch { }

        var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 3 &&
            int.TryParse(parts[0], out int r) &&
            int.TryParse(parts[1], out int g) &&
            int.TryParse(parts[2], out int b))
        {
            r = Math.Clamp(r, 0, 255);
            g = Math.Clamp(g, 0, 255);
            b = Math.Clamp(b, 0, 255);
            color = Color.FromArgb(r, g, b);
            return true;
        }

        return false;
    }

    private void Log(LogLevel level, string message)
    {
        if ((int)level >= Config.LogLevel)
            Logger.Log(level, "[C4Timer] {Message}", message);
    }

    public override void Load(bool hotReload)
    {
        RegisterEventHandler<EventBombPlanted>(OnBombPlanted);
        RegisterEventHandler<EventRoundPrestart>(OnRoundPrestart);
        RegisterEventHandler<EventBombExploded>(OnBombExploded);
        RegisterEventHandler<EventBombDefused>(OnBombDefused);
        RegisterEventHandler<EventPlayerConnectFull>(OnPlayerConnectFull);
        RegisterEventHandler<EventPlayerDisconnect>(OnPlayerDisconnect);

        AddCommand("css_c4timer_help", "Показать справку по плагину C4 Timer", OnHelpCommand);
        AddCommand("css_c4timer_settings", "Показать текущие настройки плагина", OnSettingsCommand);
        AddCommand("css_c4timer_test", "Тестовая команда для проверки работы плагина", OnTestCommand);
        AddCommand("css_c4timer_reload", "Перезагрузить конфигурацию плагина", OnReloadCommand);

        AddCommand("css_c4timer_setenabled", "Включить/выключить плагин (0/1) (по умолчанию: 1)", OnSetEnabledCommand);
        AddCommand("css_c4timer_setusedynamiccolor", "Использовать динамический цвет (0/1) (по умолчанию: 1)", OnSetUseDynamicColorCommand);
        AddCommand("css_c4timer_setcolor", "Установить фиксированный цвет (RGB/HEX/название) (по умолчанию: 255 255 255)", OnSetColorCommand);
        AddCommand("css_c4timer_settimecolor", "Установить динамические цвета (сек:цвет,сек:цвет) (по умолчанию: 20:yellow,10:red,5:darkred)", OnSetTimeColorCommand);
        AddCommand("css_c4timer_sethudchannel", "Установить канал HUD (0-255) (по умолчанию: 2)", OnSetHudChannelCommand);
        AddCommand("css_c4timer_sethudposition", "Установить позицию HUD (X Y Z) (по умолчанию: 1.3 -3.9 6.7)", OnSetHudPositionCommand);
        AddCommand("css_c4timer_setfontsize", "Установить размер шрифта (10-200) (по умолчанию: 100)", OnSetFontSizeCommand);
        AddCommand("css_c4timer_setfontname", "Установить имя шрифта (по умолчанию: Consolas)", OnSetFontNameCommand);
        AddCommand("css_c4timer_setunits", "Установить единиц на пиксель (0.001-1.0) (по умолчанию: 0.0057)", OnSetUnitsCommand);
        AddCommand("css_c4timer_setborder", "Установить обводку текста (ширина высота) (по умолчанию: 0.0 0.0)", OnSetBorderCommand);
        AddCommand("css_c4timer_setbold", "Использовать жирный шрифт (0/1) (по умолчанию: 1)", OnSetBoldCommand);
        AddCommand("css_c4timer_setmethod", "Установить метод HUD (0-телепорт, 1-point_orient) (по умолчанию: 1)", OnSetMethodCommand);
        AddCommand("css_c4timer_setexplodedsymbol", "Установить символ взрыва (например: Ⓧ) (по умолчанию: Ⓧ)", OnSetExplodedSymbolCommand);
        AddCommand("css_c4timer_setdefusedsymbol", "Установить символ обезвреживания (например: Ⓥ) (по умолчанию: Ⓥ)", OnSetDefusedSymbolCommand);
        AddCommand("css_c4timer_setloglevel", "Установить уровень логирования (0-5) (0-Trace,1-Debug,2-Info,3-Warning,4-Error,5-Critical) (по умолчанию: 4)", OnSetLogLevelCommand);

        PrintInfo();

        if (hotReload)
        {
            Server.NextFrame(() =>
            {
                foreach (var player in Utilities.GetPlayers())
                {
                    if (player != null && player.IsValid && !player.IsHLTV)
                        InitializeHUDForPlayer(player);
                }
            });
        }
    }

    public override void OnAllPluginsLoaded(bool hotReload)
    {
        try
        {
            _hudapi = IGameHUDAPI.Capability.Get();
            if (_hudapi == null)
                Log(LogLevel.Warning, "GameHUD API не загружена. Плагин не будет работать без неё.");
            else
                Log(LogLevel.Information, "GameHUD API успешно загружена.");
        }
        catch (Exception ex)
        {
            _hudapi = null;
            Log(LogLevel.Error, $"Ошибка загрузки GameHUD API: {ex.Message}");
        }
    }

    private void PrintInfo()
    {
        Log(LogLevel.Information, "===============================================");
        Log(LogLevel.Information, $"Плагин {ModuleName} версии {ModuleVersion} успешно загружен!");
        Log(LogLevel.Information, $"Автор: {ModuleAuthor}");
        Log(LogLevel.Information, "Текущие настройки:");
        Log(LogLevel.Information, $"  css_c4timer_enabled = {Config.Enabled} (по умолчанию: 1)");
        Log(LogLevel.Information, $"  css_c4timer_use_dynamic_color = {Config.UseDynamicColor} (по умолчанию: 1)");
        Log(LogLevel.Information, $"  css_c4timer_color = {Config.Color} (по умолчанию: 255 255 255)");
        Log(LogLevel.Information, $"  css_c4timer_timecolor = {Config.TimeColor} (по умолчанию: 20:yellow,10:red,5:darkred)");
        Log(LogLevel.Information, $"  css_c4timer_hud_channel = {Config.HudChannel} (по умолчанию: 2)");
        Log(LogLevel.Information, $"  css_c4timer_hud_x = {Config.HudX.ToString(CultureInfo.InvariantCulture)} (по умолчанию: 1.3)");
        Log(LogLevel.Information, $"  css_c4timer_hud_y = {Config.HudY.ToString(CultureInfo.InvariantCulture)} (по умолчанию: -3.9)");
        Log(LogLevel.Information, $"  css_c4timer_hud_z = {Config.HudZ.ToString(CultureInfo.InvariantCulture)} (по умолчанию: 6.7)");
        Log(LogLevel.Information, $"  css_c4timer_font_size = {Config.FontSize} (по умолчанию: 100)");
        Log(LogLevel.Information, $"  css_c4timer_font_name = {Config.FontName} (по умолчанию: Consolas)");
        Log(LogLevel.Information, $"  css_c4timer_units_per_px = {Config.UnitsPerPixel.ToString(CultureInfo.InvariantCulture)} (по умолчанию: 0.0057)");
        Log(LogLevel.Information, $"  css_c4timer_text_border_width = {Config.TextBorderWidth.ToString(CultureInfo.InvariantCulture)} (по умолчанию: 0.0)");
        Log(LogLevel.Information, $"  css_c4timer_text_border_height = {Config.TextBorderHeight.ToString(CultureInfo.InvariantCulture)} (по умолчанию: 0.0)");
        Log(LogLevel.Information, $"  css_c4timer_use_bold_font = {Config.UseBoldFont} (по умолчанию: 1)");
        Log(LogLevel.Information, $"  css_c4timer_hud_method = {Config.HudMethod} (по умолчанию: 1)");
        Log(LogLevel.Information, $"  css_c4timer_exploded_symbol = {Config.ExplodedSymbol} (по умолчанию: Ⓧ)");
        Log(LogLevel.Information, $"  css_c4timer_defused_symbol = {Config.DefusedSymbol} (по умолчанию: Ⓥ)");
        Log(LogLevel.Information, $"  css_c4timer_log_level = {Config.LogLevel} (0-Trace,1-Debug,2-Info,3-Warning,4-Error,5-Critical)");
        Log(LogLevel.Information, "Команды управления:");
        Log(LogLevel.Information, "  css_c4timer_help      - показать справку");
        Log(LogLevel.Information, "  css_c4timer_settings  - показать настройки");
        Log(LogLevel.Information, "  css_c4timer_test      - тестирование");
        Log(LogLevel.Information, "  css_c4timer_reload    - перезагрузить конфиг");
        Log(LogLevel.Information, "  ... и 15 команд для изменения каждой переменной конфига");
        Log(LogLevel.Information, "===============================================");
    }

    // --- Обработчики событий ---

    private HookResult OnBombPlanted(EventBombPlanted @event, GameEventInfo info)
    {
        var planted = GetPlantedC4();
        if (planted == null) return HookResult.Continue;

        _c4Planted = true;
        _c4Exploded = false;
        _c4Defused = false;
        _timerLength = (int)(planted.TimerLength + 1.0f);
        _timerCountdown = _timerLength;

        _countdownTimer?.Kill();
        _countdownTimer = new Timer(1.0f, CountdownTick, TimerFlags.REPEAT);

        UpdateC4Message();
        return HookResult.Continue;
    }

    private HookResult OnRoundPrestart(EventRoundPrestart @event, GameEventInfo info)
    {
        ResetC4();
        return HookResult.Continue;
    }

    private HookResult OnBombExploded(EventBombExploded @event, GameEventInfo info)
    {
        _c4Exploded = true;
        _c4Planted = false;
        _c4Defused = false;
        _countdownTimer?.Kill();
        _countdownTimer = null;
        UpdateC4Message();
        return HookResult.Continue;
    }

    private HookResult OnBombDefused(EventBombDefused @event, GameEventInfo info)
    {
        _c4Defused = true;
        _c4Planted = false;
        _c4Exploded = false;
        _countdownTimer?.Kill();
        _countdownTimer = null;
        UpdateC4Message();
        return HookResult.Continue;
    }

    private HookResult OnPlayerConnectFull(EventPlayerConnectFull @event, GameEventInfo info)
    {
        var player = @event.Userid;
        if (player != null && player.IsValid && !player.IsHLTV)
        {
            Server.NextFrame(() => InitializeHUDForPlayer(player));
        }
        return HookResult.Continue;
    }

    private HookResult OnPlayerDisconnect(EventPlayerDisconnect @event, GameEventInfo info)
    {
        var player = @event.Userid;
        if (player != null && player.IsValid)
        {
            int slot = player.Slot;
            _hudInitialized.Remove(slot);
            _playerCurrentColor.Remove(slot);
        }
        return HookResult.Continue;
    }

    // --- Вспомогательные методы ---

    private CPlantedC4? GetPlantedC4()
    {
        var planted = Utilities.FindAllEntitiesByDesignerName<CPlantedC4>("planted_c4");
        return planted.FirstOrDefault();
    }

    private void CountdownTick()
    {
        if (!_c4Planted || _timerCountdown <= 0)
        {
            if (_c4Planted && _timerCountdown <= 0)
            {
                _c4Exploded = true;
                _c4Planted = false;
                _countdownTimer?.Kill();
                _countdownTimer = null;
            }
            else
            {
                return;
            }
        }
        else
        {
            _timerCountdown--;
        }

        UpdateC4Message();
    }

    private void ResetC4()
    {
        _c4Planted = false;
        _c4Exploded = false;
        _c4Defused = false;
        _timerCountdown = 0;
        _timerLength = 0;
        _countdownTimer?.Kill();
        _countdownTimer = null;
        _lastMessage = "";
        ClearAllHUD();
    }

    private void UpdateC4Message()
    {
        string message = GenerateMessage();
        if (string.IsNullOrEmpty(message))
        {
            ClearAllHUD();
            _lastMessage = "";
            return;
        }

        Color currentColor = GetCurrentColor();
        if (message != _lastMessage || currentColor != _lastColor)
        {
            _lastMessage = message;
            _lastColor = currentColor;
            ShowMessageToAll(message, currentColor);
        }
    }

    private string GenerateMessage()
    {
        if (_c4Exploded)
            return $"C4:{Config.ExplodedSymbol}";

        if (_c4Defused)
            return $"C4:{Config.DefusedSymbol}";

        if (_c4Planted && _timerCountdown > 0)
            return $"C4:{_timerCountdown}";

        return "";
    }

    private Color GetCurrentColor()
    {
        if (_c4Exploded)
            return Color.Red;
        if (_c4Defused)
            return Color.Green;
        if (_c4Planted && _timerCountdown > 0)
            return GetColorForSecond(_timerCountdown);
        return Color.White;
    }

    private void ShowMessageToAll(string message, Color color)
    {
        if (_hudapi == null || Config.Enabled == 0) return;

        foreach (var player in Utilities.GetPlayers())
        {
            if (player == null || !player.IsValid || player.IsHLTV) continue;

            int slot = player.Slot;
            if (!_hudInitialized.ContainsKey(slot) || !_hudInitialized[slot])
            {
                InitializeHUDForPlayer(player);
            }

            if (_hudInitialized[slot])
            {
                if (!_playerCurrentColor.ContainsKey(slot) || _playerCurrentColor[slot] != color)
                {
                    UpdateHUDColor(player, color);
                    _playerCurrentColor[slot] = color;
                }
                _hudapi.Native_GameHUD_ShowPermanent(player, (byte)Config.HudChannel, message);
            }
        }
    }

    private void ClearAllHUD()
    {
        if (_hudapi == null) return;
        foreach (var player in Utilities.GetPlayers())
        {
            if (player == null || !player.IsValid) continue;
            _hudapi.Native_GameHUD_Remove(player, (byte)Config.HudChannel);
            _hudInitialized[player.Slot] = false;
        }
    }

    private void InitializeHUDForPlayer(CCSPlayerController player)
    {
        if (_hudapi == null || Config.Enabled == 0 || !player.IsValid) return;

        int slot = player.Slot;
        if (_hudInitialized.ContainsKey(slot) && _hudInitialized[slot])
            return;

        try
        {
            _hudapi.Native_GameHUD_Remove(player, (byte)Config.HudChannel);

            string fontName = Config.FontName;
            if (Config.UseBoldFont == 1 && !fontName.EndsWith(" Bold"))
                fontName += " Bold";

            _hudapi.Native_GameHUD_SetParams(
                player,
                (byte)Config.HudChannel,
                Config.HudX,
                Config.HudY,
                Config.HudZ,
                Color.White,
                Config.FontSize,
                fontName,
                Config.UnitsPerPixel,
                PointWorldTextJustifyHorizontal_t.POINT_WORLD_TEXT_JUSTIFY_HORIZONTAL_CENTER,
                PointWorldTextJustifyVertical_t.POINT_WORLD_TEXT_JUSTIFY_VERTICAL_CENTER,
                PointWorldTextReorientMode_t.POINT_WORLD_TEXT_REORIENT_NONE,
                Config.TextBorderHeight,
                Config.TextBorderWidth
            );

            _hudapi.Native_GameHUD_ShowPermanent(player, (byte)Config.HudChannel, " ");
            _hudInitialized[slot] = true;
        }
        catch (Exception ex)
        {
            Log(LogLevel.Error, $"Ошибка инициализации HUD для игрока {player.PlayerName}: {ex.Message}");
        }
    }

    private void UpdateHUDColor(CCSPlayerController player, Color color)
    {
        if (_hudapi == null || !player.IsValid) return;
        try
        {
            _hudapi.Native_GameHUD_UpdateParams(
                player,
                (byte)Config.HudChannel,
                Config.HudX,
                Config.HudY,
                Config.HudZ,
                color,
                Config.FontSize,
                Config.FontName + (Config.UseBoldFont == 1 ? " Bold" : ""),
                Config.UnitsPerPixel,
                PointWorldTextJustifyHorizontal_t.POINT_WORLD_TEXT_JUSTIFY_HORIZONTAL_CENTER,
                PointWorldTextJustifyVertical_t.POINT_WORLD_TEXT_JUSTIFY_VERTICAL_CENTER,
                PointWorldTextReorientMode_t.POINT_WORLD_TEXT_REORIENT_NONE,
                Config.TextBorderHeight,
                Config.TextBorderWidth
            );
        }
        catch (Exception ex)
        {
            Log(LogLevel.Error, $"Ошибка обновления цвета HUD для игрока {player.PlayerName}: {ex.Message}");
        }
    }

    private void ReinitializeAllHUD()
    {
        foreach (var player in Utilities.GetPlayers())
        {
            if (player == null || !player.IsValid || player.IsHLTV) continue;
            _hudInitialized[player.Slot] = false;
            InitializeHUDForPlayer(player);
        }
    }

    // --- Команды ---

    private void OnHelpCommand(CCSPlayerController? player, CommandInfo command)
    {
        string help = $"""
            ================================================
            СПРАВКА ПО ПЛАГИНУ {ModuleName} v{ModuleVersion}
            ================================================
            ОПИСАНИЕ:
              Отображает таймер C4 в формате "C4:30", "C4:{Config.ExplodedSymbol}" (взорвана) или "C4:{Config.DefusedSymbol}" (обезврежена).
              Цвет текста может динамически меняться в зависимости от оставшегося времени.
              Поддерживаются боты.

            КОНФИГУРАЦИОННЫЙ ФАЙЛ:
              addons/counterstrikesharp/configs/plugins/CS2_C4TimerHUD/CS2_C4TimerHUD.json

            КОМАНДЫ (значения по умолчанию в скобках):
              css_c4timer_help                - показать эту справку
              css_c4timer_settings             - показать текущие настройки
              css_c4timer_test                  - тестовая команда
              css_c4timer_reload                - перезагрузить конфигурацию

              css_c4timer_setenabled <0/1>                - вкл/выкл плагин (1)
              css_c4timer_setusedynamiccolor <0/1>        - динамический цвет (1)
              css_c4timer_setcolor <цвет>                  - фиксированный цвет (255 255 255)
              css_c4timer_settimecolor <сек:цвет,...>      - динамические цвета (20:yellow,10:red,5:darkred)
              css_c4timer_sethudchannel <0-255>            - канал HUD (2)
              css_c4timer_sethudposition <X Y Z>           - позиция HUD (1.3 -3.9 6.7)
              css_c4timer_setfontsize <10-200>             - размер шрифта (100)
              css_c4timer_setfontname <имя>                 - имя шрифта (Consolas)
              css_c4timer_setunits <0.001-1.0>              - единиц на пиксель (0.0057)
              css_c4timer_setborder <ширина высота>        - обводка текста (0.0 0.0)
              css_c4timer_setbold <0/1>                     - жирный шрифт (1)
              css_c4timer_setmethod <0/1>                   - метод HUD (1)
              css_c4timer_setexplodedsymbol <символ>       - символ взрыва (Ⓧ)
              css_c4timer_setdefusedsymbol <символ>        - символ обезвреживания (Ⓥ)
              css_c4timer_setloglevel <0-5>                 - уровень логов (4)

            ФОРМАТЫ ЦВЕТА:
              RGB: три числа от 0 до 255 через пробел (например, 255 0 0)
              HEX: #RRGGBB (например, #FF0000)
              Название: red, green, blue, yellow, white, black, darkred, orange, purple

            ПРИМЕРЫ:
              css_c4timer_setcolor 255 0 0
              css_c4timer_settimecolor 15:yellow,5:red
              css_c4timer_sethudposition 1.5 -4.0 7.0
              css_c4timer_setloglevel 2

            ВАЖНО: Для десятичных чисел используйте ТОЧКУ (.), а не запятую!
            ================================================
            """;
        command.ReplyToCommand(help);
        if (player != null)
            player.PrintToChat($" {ChatColors.Green}[C4Timer] {ChatColors.White}Справка отправлена в консоль.");
    }

    private void OnSettingsCommand(CCSPlayerController? player, CommandInfo command)
    {
        string status = _c4Exploded ? "Взорвана" : (_c4Defused ? "Обезврежена" : (_c4Planted ? $"Посажена ({_timerCountdown}с)" : "Нет"));
        string settings = $"""
            ================================================
            ТЕКУЩИЕ НАСТРОЙКИ {ModuleName} v{ModuleVersion}
            ================================================
            Плагин включен: {Config.Enabled}
            Динамический цвет: {Config.UseDynamicColor}
            Фиксированный цвет: {Config.Color}
            Динамические цвета: {Config.TimeColor}
            Канал HUD: {Config.HudChannel}
            Позиция HUD: X={Config.HudX.ToString(CultureInfo.InvariantCulture)} Y={Config.HudY.ToString(CultureInfo.InvariantCulture)} Z={Config.HudZ.ToString(CultureInfo.InvariantCulture)} (по умолчанию: 1.3 -3.9 6.7)
            Размер шрифта: {Config.FontSize} (по умолчанию 100)
            Имя шрифта: {Config.FontName}
            Единиц на пиксель: {Config.UnitsPerPixel.ToString(CultureInfo.InvariantCulture)}
            Обводка: ширина={Config.TextBorderWidth.ToString(CultureInfo.InvariantCulture)} высота={Config.TextBorderHeight.ToString(CultureInfo.InvariantCulture)}
            Жирный шрифт: {Config.UseBoldFont}
            Метод HUD: {Config.HudMethod} ({(Config.HudMethod == 1 ? "point_orient" : "teleport")})
            Символ взрыва: {Config.ExplodedSymbol}
            Символ обезвреживания: {Config.DefusedSymbol}
            Уровень логирования: {Config.LogLevel} (0-Trace,1-Debug,2-Info,3-Warning,4-Error,5-Critical)

            GameHUD API: {(_hudapi != null ? "Доступно" : "Не доступно")}
            Состояние C4: {status}
            ================================================
            """;
        command.ReplyToCommand(settings);
        if (player != null)
            player.PrintToChat($" {ChatColors.Green}[C4Timer] {ChatColors.White}Настройки отправлены в консоль.");
    }

    private void OnTestCommand(CCSPlayerController? player, CommandInfo command)
    {
        if (player == null)
        {
            command.ReplyToCommand($"[C4Timer] Эта команда доступна только игрокам.");
            return;
        }

        if (Config.Enabled == 0)
        {
            command.ReplyToCommand($"[C4Timer] Плагин выключен. Включите его командой css_c4timer_setenabled 1.");
            return;
        }

        if (_hudapi == null)
        {
            command.ReplyToCommand($"[C4Timer] GameHUD API не загружена.");
            return;
        }

        InitializeHUDForPlayer(player);
        _hudapi.Native_GameHUD_Show(player, (byte)Config.HudChannel, $"C4:30\nC4:{Config.ExplodedSymbol}\nC4:{Config.DefusedSymbol}", 5.0f);
        command.ReplyToCommand($"[C4Timer] Тестовое сообщение отправлено на канал {Config.HudChannel}.");
        Log(LogLevel.Information, $"Тестовая команда выполнена для {player.PlayerName}");
    }

    private void OnReloadCommand(CCSPlayerController? player, CommandInfo command)
    {
        try
        {
            string configPath = Path.Combine(Server.GameDirectory, "counterstrikesharp", "configs", "plugins", "CS2_C4TimerHUD", "CS2_C4TimerHUD.json");
            if (File.Exists(configPath))
            {
                string json = File.ReadAllText(configPath);
                var newConfig = System.Text.Json.JsonSerializer.Deserialize<CS2_C4TimerConfig>(json);
                if (newConfig != null)
                {
                    OnConfigParsed(newConfig);
                    SaveConfig();
                }
            }
            else
            {
                SaveConfig(); // создаст с настройками по умолчанию
            }

            ClearAllHUD();
            _lastMessage = "";
            _lastColor = Color.White;
            ReinitializeAllHUD();
            command.ReplyToCommand($"[C4Timer] Конфигурация перезагружена.");
            Log(LogLevel.Information, "Конфигурация перезагружена по команде.");
        }
        catch (Exception ex)
        {
            Log(LogLevel.Error, $"Ошибка при перезагрузке конфига: {ex.Message}");
            command.ReplyToCommand($"[C4Timer] Ошибка при перезагрузке конфига.");
        }
    }

    private void OnSetEnabledCommand(CCSPlayerController? player, CommandInfo command)
    {
        if (command.ArgCount < 2)
        {
            command.ReplyToCommand($"[C4Timer] Текущее значение Enabled: {Config.Enabled} (по умолчанию: 1). Использование: css_c4timer_setenabled <0/1>");
            return;
        }

        string arg = command.GetArg(1);
        if (int.TryParse(arg, out int value) && (value == 0 || value == 1))
        {
            int old = Config.Enabled;
            Config.Enabled = value;
            SaveConfig();
            command.ReplyToCommand($"[C4Timer] Enabled изменён с {old} на {value} (по умолчанию: 1).");
            if (value == 0) ClearAllHUD();
            else ReinitializeAllHUD();
            Log(LogLevel.Information, $"Enabled изменён на {value} (команда от {(player?.PlayerName ?? "консоли")})");
        }
        else
        {
            command.ReplyToCommand($"[C4Timer] Неверное значение. Используйте 0 или 1.");
        }
    }

    private void OnSetUseDynamicColorCommand(CCSPlayerController? player, CommandInfo command)
    {
        if (command.ArgCount < 2)
        {
            command.ReplyToCommand($"[C4Timer] Текущее значение UseDynamicColor: {Config.UseDynamicColor} (по умолчанию: 1). Использование: css_c4timer_setusedynamiccolor <0/1>");
            return;
        }

        string arg = command.GetArg(1);
        if (int.TryParse(arg, out int value) && (value == 0 || value == 1))
        {
            int old = Config.UseDynamicColor;
            Config.UseDynamicColor = value;
            SaveConfig();
            command.ReplyToCommand($"[C4Timer] UseDynamicColor изменён с {old} на {value} (по умолчанию: 1).");
            Log(LogLevel.Information, $"UseDynamicColor изменён на {value} (команда от {(player?.PlayerName ?? "консоли")})");
        }
        else
        {
            command.ReplyToCommand($"[C4Timer] Неверное значение. Используйте 0 или 1.");
        }
    }

    private void OnSetColorCommand(CCSPlayerController? player, CommandInfo command)
    {
        if (command.ArgCount < 2)
        {
            command.ReplyToCommand($"[C4Timer] Текущее значение Color: \"{Config.Color}\" (по умолчанию: 255 255 255). Использование: css_c4timer_setcolor <RGB/HEX/название>");
            return;
        }

        string newColor = command.ArgString.Trim();
        if (TryParseColor(newColor, out Color parsed))
        {
            string old = Config.Color;
            Config.Color = newColor;
            _fixedColor = parsed;
            SaveConfig();
            command.ReplyToCommand($"[C4Timer] Color изменён с \"{old}\" на \"{newColor}\" (по умолчанию: 255 255 255).");
            Log(LogLevel.Information, $"Color изменён на \"{newColor}\" (команда от {(player?.PlayerName ?? "консоли")})");
        }
        else
        {
            command.ReplyToCommand($"[C4Timer] Неверный формат цвета. Используйте RGB (255 0 0), HEX (#FF0000) или название (red).");
        }
    }

    private void OnSetTimeColorCommand(CCSPlayerController? player, CommandInfo command)
    {
        if (command.ArgCount < 2)
        {
            command.ReplyToCommand($"[C4Timer] Текущее значение TimeColor: \"{Config.TimeColor}\" (по умолчанию: 20:yellow,10:red,5:darkred). Использование: css_c4timer_settimecolor <сек:цвет,сек:цвет,...>");
            return;
        }

        string newTimeColor = command.ArgString.Trim();
        string old = Config.TimeColor;
        Config.TimeColor = newTimeColor;
        ParseTimeColorConfig(Config.TimeColor);
        SaveConfig();
        command.ReplyToCommand($"[C4Timer] TimeColor изменён с \"{old}\" на \"{newTimeColor}\" (по умолчанию: 20:yellow,10:red,5:darkred).");
        Log(LogLevel.Information, $"TimeColor изменён на \"{newTimeColor}\" (команда от {(player?.PlayerName ?? "консоли")})");
    }

    private void OnSetHudChannelCommand(CCSPlayerController? player, CommandInfo command)
    {
        if (command.ArgCount < 2)
        {
            command.ReplyToCommand($"[C4Timer] Текущее значение HudChannel: {Config.HudChannel} (по умолчанию: 2). Использование: css_c4timer_sethudchannel <0-255>");
            return;
        }

        string arg = command.GetArg(1);
        if (int.TryParse(arg, out int value))
        {
            int old = Config.HudChannel;
            Config.HudChannel = Math.Clamp(value, 0, 255);
            SaveConfig();
            ClearAllHUD();
            ReinitializeAllHUD();
            command.ReplyToCommand($"[C4Timer] HudChannel изменён с {old} на {Config.HudChannel} (по умолчанию: 2).");
            Log(LogLevel.Information, $"HudChannel изменён на {Config.HudChannel} (команда от {(player?.PlayerName ?? "консоли")})");
        }
        else
        {
            command.ReplyToCommand($"[C4Timer] Неверное значение. Используйте число от 0 до 255.");
        }
    }

    private void OnSetHudPositionCommand(CCSPlayerController? player, CommandInfo command)
    {
        if (command.ArgCount < 4)
        {
            command.ReplyToCommand($"[C4Timer] Текущая позиция: X={Config.HudX.ToString(CultureInfo.InvariantCulture)}, Y={Config.HudY.ToString(CultureInfo.InvariantCulture)}, Z={Config.HudZ.ToString(CultureInfo.InvariantCulture)} (по умолчанию: 1.3 -3.9 6.7). Использование: css_c4timer_sethudposition <X Y Z>");
            return;
        }

        string argX = command.GetArg(1).Replace(',', '.');
        string argY = command.GetArg(2).Replace(',', '.');
        string argZ = command.GetArg(3).Replace(',', '.');

        if (float.TryParse(argX, NumberStyles.Float, CultureInfo.InvariantCulture, out float x) &&
            float.TryParse(argY, NumberStyles.Float, CultureInfo.InvariantCulture, out float y) &&
            float.TryParse(argZ, NumberStyles.Float, CultureInfo.InvariantCulture, out float z))
        {
            float oldX = Config.HudX;
            float oldY = Config.HudY;
            float oldZ = Config.HudZ;

            Config.HudX = Math.Clamp(x, -100.0f, 100.0f);
            Config.HudY = Math.Clamp(y, -100.0f, 100.0f);
            Config.HudZ = Math.Clamp(z, 0.0f, 200.0f);

            SaveConfig();
            ReinitializeAllHUD();
            command.ReplyToCommand($"[C4Timer] Позиция изменена с ({oldX.ToString(CultureInfo.InvariantCulture)},{oldY.ToString(CultureInfo.InvariantCulture)},{oldZ.ToString(CultureInfo.InvariantCulture)}) на ({Config.HudX.ToString(CultureInfo.InvariantCulture)},{Config.HudY.ToString(CultureInfo.InvariantCulture)},{Config.HudZ.ToString(CultureInfo.InvariantCulture)}) (по умолчанию: 1.3 -3.9 6.7).");
            Log(LogLevel.Information, $"Позиция HUD изменена на {Config.HudX},{Config.HudY},{Config.HudZ} (команда от {(player?.PlayerName ?? "консоли")})");
        }
        else
        {
            command.ReplyToCommand($"[C4Timer] Неверные координаты. Используйте числа с точкой, например: 1.3 -3.9 6.7");
        }
    }

    private void OnSetFontSizeCommand(CCSPlayerController? player, CommandInfo command)
    {
        if (command.ArgCount < 2)
        {
            command.ReplyToCommand($"[C4Timer] Текущее значение FontSize: {Config.FontSize} (по умолчанию: 100). Использование: css_c4timer_setfontsize <10-200>");
            return;
        }

        string arg = command.GetArg(1);
        if (int.TryParse(arg, out int value))
        {
            int old = Config.FontSize;
            Config.FontSize = Math.Clamp(value, 10, 200);
            SaveConfig();
            ReinitializeAllHUD();
            command.ReplyToCommand($"[C4Timer] FontSize изменён с {old} на {Config.FontSize} (по умолчанию: 100).");
            Log(LogLevel.Information, $"FontSize изменён на {Config.FontSize} (команда от {(player?.PlayerName ?? "консоли")})");
        }
        else
        {
            command.ReplyToCommand($"[C4Timer] Неверное значение. Используйте число от 10 до 200.");
        }
    }

    private void OnSetFontNameCommand(CCSPlayerController? player, CommandInfo command)
    {
        if (command.ArgCount < 2)
        {
            command.ReplyToCommand($"[C4Timer] Текущее значение FontName: \"{Config.FontName}\" (по умолчанию: Consolas). Использование: css_c4timer_setfontname <имя>");
            return;
        }

        string old = Config.FontName;
        string value = command.GetArg(1);
        if (!string.IsNullOrWhiteSpace(value))
        {
            Config.FontName = value;
            SaveConfig();
            ReinitializeAllHUD();
            command.ReplyToCommand($"[C4Timer] FontName изменён с \"{old}\" на \"{Config.FontName}\" (по умолчанию: Consolas).");
            Log(LogLevel.Information, $"FontName изменён на \"{Config.FontName}\" (команда от {(player?.PlayerName ?? "консоли")})");
        }
        else
        {
            command.ReplyToCommand($"[C4Timer] Имя шрифта не может быть пустым.");
        }
    }

    private void OnSetUnitsCommand(CCSPlayerController? player, CommandInfo command)
    {
        if (command.ArgCount < 2)
        {
            command.ReplyToCommand($"[C4Timer] Текущее значение UnitsPerPixel: {Config.UnitsPerPixel.ToString(CultureInfo.InvariantCulture)} (по умолчанию: 0.0057). Использование: css_c4timer_setunits <0.001-1.0>");
            return;
        }

        string arg = command.GetArg(1).Replace(',', '.');
        if (float.TryParse(arg, NumberStyles.Float, CultureInfo.InvariantCulture, out float value))
        {
            float old = Config.UnitsPerPixel;
            Config.UnitsPerPixel = Math.Clamp(value, 0.001f, 1.0f);
            SaveConfig();
            ReinitializeAllHUD();
            command.ReplyToCommand($"[C4Timer] UnitsPerPixel изменён с {old.ToString(CultureInfo.InvariantCulture)} на {Config.UnitsPerPixel.ToString(CultureInfo.InvariantCulture)} (по умолчанию: 0.0057).");
            Log(LogLevel.Information, $"UnitsPerPixel изменён на {Config.UnitsPerPixel} (команда от {(player?.PlayerName ?? "консоли")})");
        }
        else
        {
            command.ReplyToCommand($"[C4Timer] Неверное значение. Используйте число с точкой, например 0.0057.");
        }
    }

    private void OnSetBorderCommand(CCSPlayerController? player, CommandInfo command)
    {
        if (command.ArgCount < 3)
        {
            command.ReplyToCommand($"[C4Timer] Текущая обводка: ширина={Config.TextBorderWidth.ToString(CultureInfo.InvariantCulture)}, высота={Config.TextBorderHeight.ToString(CultureInfo.InvariantCulture)} (по умолчанию: 0.0 0.0). Использование: css_c4timer_setborder <ширина> <высота>");
            return;
        }

        string argW = command.GetArg(1).Replace(',', '.');
        string argH = command.GetArg(2).Replace(',', '.');

        if (float.TryParse(argW, NumberStyles.Float, CultureInfo.InvariantCulture, out float w) &&
            float.TryParse(argH, NumberStyles.Float, CultureInfo.InvariantCulture, out float h))
        {
            float oldW = Config.TextBorderWidth;
            float oldH = Config.TextBorderHeight;
            Config.TextBorderWidth = Math.Clamp(w, 0.0f, 10.0f);
            Config.TextBorderHeight = Math.Clamp(h, 0.0f, 10.0f);
            SaveConfig();
            ReinitializeAllHUD();
            command.ReplyToCommand($"[C4Timer] Обводка изменена с ({oldW.ToString(CultureInfo.InvariantCulture)},{oldH.ToString(CultureInfo.InvariantCulture)}) на ({Config.TextBorderWidth.ToString(CultureInfo.InvariantCulture)},{Config.TextBorderHeight.ToString(CultureInfo.InvariantCulture)}) (по умолчанию: 0.0 0.0).");
            Log(LogLevel.Information, $"Обводка изменена на {Config.TextBorderWidth},{Config.TextBorderHeight} (команда от {(player?.PlayerName ?? "консоли")})");
        }
        else
        {
            command.ReplyToCommand($"[C4Timer] Неверные значения. Используйте числа с точкой, например 0.0 0.0.");
        }
    }

    private void OnSetBoldCommand(CCSPlayerController? player, CommandInfo command)
    {
        if (command.ArgCount < 2)
        {
            command.ReplyToCommand($"[C4Timer] Текущее значение UseBoldFont: {Config.UseBoldFont} (по умолчанию: 1). Использование: css_c4timer_setbold <0/1>");
            return;
        }

        string arg = command.GetArg(1);
        if (int.TryParse(arg, out int value) && (value == 0 || value == 1))
        {
            int old = Config.UseBoldFont;
            Config.UseBoldFont = value;
            SaveConfig();
            ReinitializeAllHUD();
            command.ReplyToCommand($"[C4Timer] UseBoldFont изменён с {old} на {value} (по умолчанию: 1).");
            Log(LogLevel.Information, $"UseBoldFont изменён на {value} (команда от {(player?.PlayerName ?? "консоли")})");
        }
        else
        {
            command.ReplyToCommand($"[C4Timer] Неверное значение. Используйте 0 или 1.");
        }
    }

    private void OnSetMethodCommand(CCSPlayerController? player, CommandInfo command)
    {
        if (command.ArgCount < 2)
        {
            command.ReplyToCommand($"[C4Timer] Текущее значение HudMethod: {Config.HudMethod} (по умолчанию: 1). Использование: css_c4timer_setmethod <0|1>");
            return;
        }

        string arg = command.GetArg(1);
        if (int.TryParse(arg, out int value) && (value == 0 || value == 1))
        {
            int old = Config.HudMethod;
            Config.HudMethod = value;
            SaveConfig();
            ClearAllHUD();
            ReinitializeAllHUD();
            command.ReplyToCommand($"[C4Timer] HudMethod изменён с {old} на {value} ({(value == 1 ? "point_orient" : "teleport")}) (по умолчанию: 1).");
            Log(LogLevel.Information, $"HudMethod изменён на {value} (команда от {(player?.PlayerName ?? "консоли")})");
        }
        else
        {
            command.ReplyToCommand($"[C4Timer] Неверное значение. Используйте 0 (телепорт) или 1 (point_orient).");
        }
    }

    private void OnSetExplodedSymbolCommand(CCSPlayerController? player, CommandInfo command)
    {
        if (command.ArgCount < 2)
        {
            command.ReplyToCommand($"[C4Timer] Текущий символ взрыва: \"{Config.ExplodedSymbol}\" (по умолчанию: Ⓧ). Использование: css_c4timer_setexplodedsymbol <символ>");
            return;
        }

        string newSymbol = command.GetArg(1);
        string old = Config.ExplodedSymbol;
        Config.ExplodedSymbol = newSymbol;
        SaveConfig();
        command.ReplyToCommand($"[C4Timer] Символ взрыва изменён с \"{old}\" на \"{newSymbol}\" (по умолчанию: Ⓧ).");
        if (_c4Exploded) UpdateC4Message();
        Log(LogLevel.Information, $"Символ взрыва изменён на \"{newSymbol}\" (команда от {(player?.PlayerName ?? "консоли")})");
    }

    private void OnSetDefusedSymbolCommand(CCSPlayerController? player, CommandInfo command)
    {
        if (command.ArgCount < 2)
        {
            command.ReplyToCommand($"[C4Timer] Текущий символ обезвреживания: \"{Config.DefusedSymbol}\" (по умолчанию: Ⓥ). Использование: css_c4timer_setdefusedsymbol <символ>");
            return;
        }

        string newSymbol = command.GetArg(1);
        string old = Config.DefusedSymbol;
        Config.DefusedSymbol = newSymbol;
        SaveConfig();
        command.ReplyToCommand($"[C4Timer] Символ обезвреживания изменён с \"{old}\" на \"{newSymbol}\" (по умолчанию: Ⓥ).");
        if (_c4Defused) UpdateC4Message();
        Log(LogLevel.Information, $"Символ обезвреживания изменён на \"{newSymbol}\" (команда от {(player?.PlayerName ?? "консоли")})");
    }

    private void OnSetLogLevelCommand(CCSPlayerController? player, CommandInfo command)
    {
        if (command.ArgCount < 2)
        {
            command.ReplyToCommand($"[C4Timer] Текущий уровень логов: {Config.LogLevel} (0-Trace,1-Debug,2-Info,3-Warning,4-Error,5-Critical) (по умолчанию: 4). Использование: css_c4timer_setloglevel <0-5>");
            return;
        }

        string arg = command.GetArg(1);
        if (int.TryParse(arg, out int value) && value >= 0 && value <= 5)
        {
            int old = Config.LogLevel;
            Config.LogLevel = value;
            SaveConfig();
            command.ReplyToCommand($"[C4Timer] Уровень логов изменён с {old} на {value} (по умолчанию: 4).");
            Log(LogLevel.Information, $"LogLevel изменён на {value} (команда от {(player?.PlayerName ?? "консоли")})");
        }
        else
        {
            command.ReplyToCommand($"[C4Timer] Неверное значение. Используйте число от 0 до 5.");
        }
    }

    private void SaveConfig()
    {
        try
        {
            string configPath = Path.Combine(Server.GameDirectory, "counterstrikesharp", "configs", "plugins", "CS2_C4TimerHUD", "CS2_C4TimerHUD.json");
            Directory.CreateDirectory(Path.GetDirectoryName(configPath)!);
            var options = new System.Text.Json.JsonSerializerOptions { WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
            string json = System.Text.Json.JsonSerializer.Serialize(Config, options);
            File.WriteAllText(configPath, json);
            Log(LogLevel.Debug, $"Конфигурация сохранена в {configPath}");
        }
        catch (Exception ex)
        {
            Log(LogLevel.Error, $"Ошибка сохранения конфига: {ex.Message}");
        }
    }

    public override void Unload(bool hotReload)
    {
        _countdownTimer?.Kill();
        ClearAllHUD();
        Log(LogLevel.Information, "Плагин выгружен.");
    }
}