using System;
using System.Collections.Generic;
using System.Text;
using PanasonicSerialCommon;
using PanasonicSerialServer.Interfaces;
using PanasonicSerialServer.PanasonicCommands;
using Serilog;

namespace PanasonicSerialServer
{
    public class Commands
    {
        public const string
            PowerOn = "PowerOn",
            PowerOff = "PowerOff",
            SwitchInput = "SwitchInput",
            Menu = "Menu",
            Enter = "Enter",
            Return = "Return",
            Up = "Up",
            Down = "Down",
            Left = "Left",
            Right = "Right",
            Lens = "Lens",
            Default = "Default",
            Freeze = "Freeze",
            FunctionButton = "FunctionButton",
            Blank = "Blank",
            Picture = "Picture",
            MemoryLoad = "MemoryLoad",
            Waveform = "Waveform",
            Aspect = "Aspec",
            PictureMode = "PictureMode",
            PictureAdjust = "PictureAdjust",
            ColourManagement = "ColourManagement",
            VieraLink = "VieraLink",
            SubMenu = "SubMenu",
            Keystone = "Keystone",
            AdvancedMenu = "AdvancedMenu",
            AutoSetup = "AutoSetup",
            Sleep = "Sleep",
            Settings3D = "Settings3D",
            LensMemoryLoad = "LensMemoryLoad",
            GammaAdjustment = "GammaAdjustment",
            Trigger1 = "Trigger1",
            Trigger2 = "Trigger2",
            InputFormat3D = "InputFormat3D";

        private static Dictionary<string, Func<ServerConfig, IPanasonicCommand>> list = new Dictionary<string, Func<ServerConfig, IPanasonicCommand>>()
        {
            { PowerOn,          (_) => new PanasonicCommand(PowerOn,          "PON", false, 9) },
            { PowerOff,         (_) => new PanasonicCommand(PowerOff,         "POF", false, 20) },
            { SwitchInput,      (_) => new PanasonicCommand(SwitchInput,      "IIS", true) },
            { Menu,             (_) => new PanasonicCommand(Menu,             "OMN", false) },
            { Enter,            (_) => new PanasonicCommand(Enter,            "OEN", false) },
            { Return,           (_) => new PanasonicCommand(Return,           "OBK", false) },
            { Up,               (_) => new PanasonicCommand(Up,               "OCU", false) },
            { Down,             (_) => new PanasonicCommand(Down,             "OCD", false) },
            { Left,             (_) => new PanasonicCommand(Left,             "OCL", false) },
            { Right,            (_) => new PanasonicCommand(Right,            "OCR", false) },
            { Lens,             (_) => new PanasonicCommand(Lens,             "OLE", false) },
            { Default,          (_) => new PanasonicCommand(Default,          "OST", false) },
            { Freeze,           (_) => new PanasonicCommand(Freeze,           "OFZ", true) },
            { FunctionButton,   (_) => new PanasonicCommand(FunctionButton,   "FC1", false) },
            { Blank,            (_) => new PanasonicCommand(Blank,            "OSH", false) },
            { Picture,          (_) => new PanasonicCommand(Picture,          "OVM", false) },
            { MemoryLoad,       (_) => new PanasonicCommand(MemoryLoad,       "OMM", false) },
            { Waveform,         (_) => new PanasonicCommand(Waveform,         "OWM", true) },
            { Aspect,           (_) => new PanasonicCommand(Aspect,           "VS1", false) },
            { PictureMode,      (_) => new PanasonicCommand(PictureMode,      "VPM", true) },
            { PictureAdjust,    (_) => new PanasonicCommand(PictureAdjust,    "DPA", false) },
            { ColourManagement, (_) => new PanasonicCommand(ColourManagement, "DCM", false) },
            { VieraLink,        (_) => new PanasonicCommand(VieraLink,        "OVL", false) },
            { SubMenu,          (_) => new PanasonicCommand(SubMenu,          "OSM", false) },
            { Keystone,         (_) => new PanasonicCommand(Keystone,         "KST", false) },
            { AdvancedMenu,     (_) => new PanasonicCommand(AdvancedMenu,     "DAM", false) },
            { AutoSetup,        (_) => new PanasonicCommand(AutoSetup,        "OAS", false) },
            { Sleep,            (_) => new PanasonicCommand(Sleep,            "OOT", true) },
            { Settings3D,       (_) => new PanasonicCommand(Settings3D,       "O3D", false) },
            { LensMemoryLoad,   (config) => new PanasonicVxxLensCommand(LensMemoryLoad, config.LensAspectRatios, 9) },
            { GammaAdjustment,  (_) => new PanasonicVxxCommand(GammaAdjustment) },
            { Trigger1,         (_) => new PanasonicVxxCommand(Trigger1) },
            { Trigger2,         (_) => new PanasonicVxxCommand(Trigger2) },
            { InputFormat3D,    (_) => new PanasonicVxxCommand(InputFormat3D) },

        };

        public static IPanasonicCommand GetCommand(Message message, ServerConfig config)
        {
            IPanasonicCommand command = list.TryGetValue(message.Command, out Func<ServerConfig, IPanasonicCommand> func) ? func(config) : null;

            if (null != command)
            {
                if (command.HasOption)
                {
                    command.Option = message.Option;
                }
            }
            else
            {
                Log.Information("No command called [{Command}] found", message.Command);
            }

            return command;
        }
    }
}
