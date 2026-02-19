# CS2_C4TimerHUD
Плагин отображает таймер C4 в центре экрана (3D HUD) в формате "C4:30", "C4:Ⓧ" (взорвана) или "C4:Ⓥ" (обезврежена). Цвет текста может динамически меняться в зависимости от оставшегося времени (настраивается). Использует стабильный метод point_orient. 

# Требования
~~~
CounterStrikeSharp API версии 362 или выше
.NET 8.0 Runtime
Плагин GameHUD (для отображения 3D текста)
~~~

# Конфигурационные параметры
~~~
css_c4timer_enabled <0/1>, def.=1 – Включение/выключение плагина.
css_c4timer_use_dynamic_color <0/1>, def.=1 – Использовать динамический цвет (1) или фиксированный цвет из параметра color (0).
css_c4timer_color (строка), def.="255 255 255" – Фиксированный цвет текста, если динамический цвет выключен. Поддерживаются форматы: RGB (три числа 0-255 через пробел, например "255 0 0"), HEX ("#FF0000"), название цвета (red, green, blue, yellow, white, black, darkred, orange, purple).
css_c4timer_timecolor (строка), def.="20:yellow,10:red,5:darkred" – Динамические цвета для секунд. Формат: секунда:цвет,секунда:цвет,... Цвет применяется для всех секунд от 0 до указанной. Например, "20:yellow,10:red,5:darkred" означает: от 20 до 11 – жёлтый, от 10 до 6 – красный, от 5 до 0 – тёмно-красный.
css_c4timer_hud_channel <0-255>, def.=2 – Канал HUD для отображения (должен отличаться от каналов других плагинов, использующих GameHUD).
css_c4timer_hud_x <-100.0 до 100.0>, def.=1.3 – Позиция HUD по оси X.
css_c4timer_hud_y <-100.0 до 100.0>, def.=-3.9 – Позиция HUD по оси Y.
css_c4timer_hud_z <0.0-200.0>, def.=6.7 – Позиция HUD по оси Z (высота).
css_c4timer_font_size <10-200>, def.=100 – Размер шрифта.
css_c4timer_font_name (строка), def.="Consolas" – Имя шрифта.
css_c4timer_units_per_px <0.001-1.0>, def.=0.0057 – Единиц на пиксель для масштабирования шрифта.
css_c4timer_text_border_width <0.0-10.0>, def.=0.0 – Толщина обводки текста (0 = нет обводки).
css_c4timer_text_border_height <0.0-10.0>, def.=0.0 – Высота обводки текста.
css_c4timer_use_bold_font <0/1>, def.=1 – Использовать жирное начертание шрифта.
css_c4timer_hud_method <0/1>, def.=1 – Метод отображения HUD: 1 – point_orient (стабильный, без тряски), 0 – teleport (быстрый, но может трястись).
css_c4timer_exploded_symbol (строка), def.="Ⓧ" – Символ, отображаемый после взрыва C4 (например, "C4:Ⓧ").
css_c4timer_defused_symbol (строка), def.="Ⓥ" – Символ, отображаемый после обезвреживания C4 (например, "C4:Ⓥ").
css_c4timer_log_level <0-5>, def.=4 – Уровень логирования (0-Trace,1-Debug,2-Info,3-Warning,4-Error,5-Critical).
~~~

# Консольные команды
~~~
css_c4timer_help – Показать подробную справку по плагину.
css_c4timer_settings – Показать текущие настройки и состояние C4.
css_c4timer_test – Отправить тестовое сообщение в HUD (доступно только игроку).
css_c4timer_reload – Перезагрузить конфигурацию из файла и переинициализировать HUD для всех игроков.
css_c4timer_setenabled <0/1> – Установить css_c4timer_enabled.
css_c4timer_setusedynamiccolor <0/1> – Установить css_c4timer_use_dynamic_color.
css_c4timer_setcolor <цвет> – Установить фиксированный цвет (css_c4timer_color). Форматы: RGB ("255 0 0"), HEX ("#FF0000"), название ("red").
css_c4timer_settimecolor <сек:цвет,сек:цвет,...> – Установить динамические цвета (css_c4timer_timecolor). Например: "15:yellow,5:red".
css_c4timer_sethudchannel <0-255> – Установить css_c4timer_hud_channel.
css_c4timer_sethudposition <X Y Z> – Установить позицию HUD (X, Y, Z через пробел, дробная часть через точку).
css_c4timer_setfontsize <10-200> – Установить css_c4timer_font_size.
css_c4timer_setfontname <имя> – Установить css_c4timer_font_name.
css_c4timer_setunits <0.001-1.0> – Установить css_c4timer_units_per_px.
css_c4timer_setborder <ширина> <высота> – Установить обводку текста (ширина и высота через пробел).
css_c4timer_setbold <0/1> – Установить css_c4timer_use_bold_font.
css_c4timer_setmethod <0/1> – Установить css_c4timer_hud_method (0 – teleport, 1 – point_orient).
css_c4timer_setexplodedsymbol <символ> – Установить css_c4timer_exploded_symbol.
css_c4timer_setdefusedsymbol <символ> – Установить css_c4timer_defused_symbol.
css_c4timer_setloglevel <0-5> – Установить css_c4timer_log_level.
~~~

# ЭТОТ ПЛАГИН ФОРК ЭТОГО ПЛАГИНА:

https://github.com/R0mz1k/css-C4-Timer
