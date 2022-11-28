using UI_DSM.Client.Enumerator;

namespace UI_DSM.Client.Extensions
{
    public static class EnumerationExtensions
    {
        public static string ToColorString(this InterfaceCategory category)
        {
            switch (category)
            {
                case InterfaceCategory.Other: return "gray";
                case InterfaceCategory.Power_Interfaces: return "red";
                case InterfaceCategory.Signal_Interfaces: return "#00B0F0";
                case InterfaceCategory.TM_TC_Interfaces: return "green";
                case InterfaceCategory.DataBus_Interfaces: return "yellow";
                case InterfaceCategory.Str_Interfaces: return "black";
                case InterfaceCategory.TC_Interfaces: return "#843C0C";
                case InterfaceCategory.Mechanisms_Interfaces: return "#7030A0";
                case InterfaceCategory.Prop_Interfaces: return "#FFC000";
                case InterfaceCategory.Comms_Interfaces: return "#99FFCC";
                default: throw new NotImplementedException();
            }
        }

        public static string ToName(this InterfaceCategory category)
        {
            switch (category)
            {
                case InterfaceCategory.Other: return "Other";
                case InterfaceCategory.Power_Interfaces: return "Power";
                case InterfaceCategory.Signal_Interfaces: return "Signal";
                case InterfaceCategory.TM_TC_Interfaces: return "Tele-metry/command";
                case InterfaceCategory.DataBus_Interfaces: return "Data Bus";
                case InterfaceCategory.Str_Interfaces: return "Structural";
                case InterfaceCategory.TC_Interfaces: return "Thermal Control";
                case InterfaceCategory.Mechanisms_Interfaces: return "Mechanisms";
                case InterfaceCategory.Prop_Interfaces: return "Propulsion";
                case InterfaceCategory.Comms_Interfaces: return "Communications";
                default: throw new NotImplementedException();
            }
        }
    }
}
