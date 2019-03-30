using System;
using System.Management;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Timers;
using System.Runtime.InteropServices;

namespace CSharp_Windows_Shutdown
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        private class Item
        {
            public string Name;
            public int Value;
            public Item(string name, int value)
            {
                Name = name; Value = value;
            }
            public override string ToString()
            {
                return Name;
            }
        }

        public MainWindow()
        {


            InitializeComponent();
            // ComboBox Items
            cb1.Items.Add(new Item("Benutzer wechseln", 1));
            cb1.Items.Add(new Item("Abmelden", 2));
            cb1.Items.Add(new Item("Energie sparen", 3));
            cb1.Items.Add(new Item("Herunterfahren", 4));
            cb1.Items.Add(new Item("Neu starten", 5));

            // ComboBox Pre-Selected
            cb1.SelectedIndex = 3;
            desc.Text = "Alle Apps werden geschlossen, und der PC wird ausgeschaltet.";




        }

        // TIMER //

        // Timer fürs Herunterfahren
        private static System.Timers.Timer sdtimer;
        private static void SetShutdownTimer()
        {
            sdtimer = new System.Timers.Timer(10000);
            sdtimer.Elapsed += ShutdownTimedEvent;
            sdtimer.AutoReset = false;
            sdtimer.Enabled = true;
        }

        private static void ShutdownTimedEvent(Object source, ElapsedEventArgs e)
        {
            ShutdownPC();
        }

        // Timer fürs Neustarten
        private static System.Timers.Timer rstimer;
        private static void SetRestartTimer()
        {
            rstimer = new System.Timers.Timer(10000);
            rstimer.Elapsed += RestartTimedEvent;
            rstimer.AutoReset = false;
            rstimer.Enabled = true;
        }
        private static void RestartTimedEvent(Object source, ElapsedEventArgs e)
        {
            RestartPC();
        }

        // Timer für Abmelden
        private static System.Timers.Timer sotimer;
        private static void SetSignOutTimer()
        {
            sotimer = new System.Timers.Timer(7000);
            sotimer.Elapsed += SignOutTimedEvent;
            sotimer.AutoReset = false;
            sotimer.Enabled = true;
        }

        private static void SignOutTimedEvent(Object source, ElapsedEventArgs e)
        {
            SignOutPC();
        }

        // HERUNTERFAHREN FUNKTION //
        public static void ShutdownPC()
        {
            ManagementBaseObject outParameters = null;
            ManagementClass WinOS = new ManagementClass("Win32_OperatingSystem");
            WinOS.Get();
            WinOS.Scope.Options.EnablePrivileges = true;
            ManagementBaseObject inParameters = WinOS.GetMethodParameters("Win32Shutdown");
            inParameters["flags"] = "1";
            inParameters["Reserved"] = "0";
            foreach (ManagementObject manObj in WinOS.GetInstances())
            {
                outParameters = manObj.InvokeMethod("Win32Shutdown", inParameters, null);
            }
        }

        private void cancel_shutdown()
        {
            sdtimer.Stop();
            desc.Text = "Der Vorgang wurde erfolgreich abgebrochen";
            cb1.IsEnabled = true;
        }

        // NEUSTARTEN FUNKTION //
        public static void RestartPC()
        {
            ManagementBaseObject outParameters = null;
            ManagementClass WinOS = new ManagementClass("Win32_OperatingSystem");
            WinOS.Get();
            WinOS.Scope.Options.EnablePrivileges = true;
            ManagementBaseObject inParameters = WinOS.GetMethodParameters("Win32Shutdown");
            inParameters["flags"] = "2";
            inParameters["Reserved"] = "0";
            foreach (ManagementObject manObj in WinOS.GetInstances())
            {
                outParameters = manObj.InvokeMethod("Win32Shutdown", inParameters, null);
            }
        }
        private void cancel_restart()
        {
            rstimer.Stop();
            desc.Text = "Der Vorgang wurde erfolgreich abgebrochen";
            cb1.IsEnabled = true;
        }

        // ABMELDEN FUNKTION //
        public static void SignOutPC()
        {
            ManagementBaseObject outParameters = null;
            ManagementClass WinOS = new ManagementClass("Win32_OperatingSystem");
            WinOS.Get();
            WinOS.Scope.Options.EnablePrivileges = true;
            ManagementBaseObject inParameters = WinOS.GetMethodParameters("Win32Shutdown");
            inParameters["flags"] = "0";
            inParameters["Reserved"] = "1";
            foreach (ManagementObject manObj in WinOS.GetInstances())
            {
                outParameters = manObj.InvokeMethod("Win32Shutdown", inParameters, null);
            }
        }

        private void cancel_signout()
        {
            sotimer.Stop();
            desc.Text = "Der Vorgang wurde erfolgreich abgebrochen";
            cb1.IsEnabled = true;
        }


        [DllImport("Powrprof.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool SetSuspendState(bool hibernate, bool forceCritical, bool disableWakeEvenet);

        [DllImport("user32.dll")]
        public static extern void LockWorkStation();


        private void Btn_close_Click(object sender, RoutedEventArgs e)
        {
            if (desc.Text == "Der Vorgang wurde erfolgreich abgebrochen")
                Close();

            if (desc.Text == "Der PC wird in 10 Sekunden heruntergefahren. Drücken sie auf 'Abbrechen' um den Vorgang abzubrechen oder nochmals auf 'OK' um den PC direkt heruterzufahren.")
                cancel_shutdown();

            if (desc.Text == "Der PC wird in 10 Sekunden neugestartet. Drücken sie auf 'Abbrechen' um den Vorgang abzubrechen oder nochmals auf 'OK' um den PC direkt neuzustarten.")
                cancel_restart();

            if (desc.Text == "Sie werden in 7 Sekunden abgemeldet. Drücken sie auf 'Abbrechen' um den Vorgang abzubrechen oder nochmals auf 'OK' um den sich direkt abzumelden.")
                cancel_signout();

            if (desc.Text == "Wechselt Benutzer, ohne Anwendungen zu schließen.")
                Close();

            if (desc.Text == "Alle Anwendungen werden geschlossen, und Sie werden abgemeldet")
                Close();

            if (desc.Text == "Der PC bleibt eingeschaltet, verbraucht aber wenig Strom. Apps bleiben geöffnet, sodass Sie beim Reaktivieren des PCs sofort dort weitermachen könne, wo Sie aufgehört haben.")
                Close();

            if (desc.Text == "Alle Apps werden geschlossen, und der PC wird ausgeschaltet.")
                Close();

            if (desc.Text == "Alle Apps werden geschlossen, und der PC wird aus- und dann wieder eingeschaltet.")
                Close();

        }

        private void Cb1_DropDownClosed(object sender, EventArgs e)
        {

            // ComboBox, Text Aenderung
            if (cb1.SelectedIndex == 0)
                desc.Text = "Wechselt Benutzer, ohne Anwendungen zu schließen.";
            if (cb1.SelectedIndex == 1)
                desc.Text = "Alle Anwendungen werden geschlossen, und Sie werden abgemeldet";
            if (cb1.SelectedIndex == 2)
                desc.Text = "Der PC bleibt eingeschaltet, verbraucht aber wenig Strom. Apps bleiben geöffnet, sodass Sie beim Reaktivieren des PCs sofort dort weitermachen könne, wo Sie aufgehört haben.";
            if (cb1.SelectedIndex == 3)
                desc.Text = "Alle Apps werden geschlossen, und der PC wird ausgeschaltet.";
            if (cb1.SelectedIndex == 4)
                desc.Text = "Alle Apps werden geschlossen, und der PC wird aus- und dann wieder eingeschaltet.";

        }

        private void Btn_info_Click(object sender, RoutedEventArgs e)
        {
            cb1.SelectedIndex = -1;
            desc.Text = "INFO: Dieses Programm wurde mit C# (CSharp) in Visual Studio 2019 programmiert. Vielen Dank für die Nutzung dieses Programmes.";
        }

        private void Btn_ok_Click(object sender, RoutedEventArgs e)
        {
            // Herunterfahren
            if (desc.Text == "Der PC wird in 10 Sekunden heruntergefahren. Drücken sie auf 'Abbrechen' um den Vorgang abzubrechen oder nochmals auf 'OK' um den PC direkt heruterzufahren.")
                ShutdownPC();
            if (cb1.SelectedIndex == 3)
                SetShutdownTimer();
            if (cb1.SelectedIndex == 3)
                desc.Text = "Der PC wird in 10 Sekunden heruntergefahren. Drücken sie auf 'Abbrechen' um den Vorgang abzubrechen oder nochmals auf 'OK' um den PC direkt heruterzufahren.";
            if (cb1.SelectedIndex == 3)
                cb1.IsEnabled = false;
            // Neustarten
            if (desc.Text == "Der PC wird in 10 Sekunden neugestartet. Drücken sie auf 'Abbrechen' um den Vorgang abzubrechen oder nochmals auf 'OK' um den PC direkt neuzustarten.")
                RestartPC();
            if (cb1.SelectedIndex == 4)
                SetRestartTimer();
            if (cb1.SelectedIndex == 4)
                desc.Text = "Der PC wird in 10 Sekunden neugestartet. Drücken sie auf 'Abbrechen' um den Vorgang abzubrechen oder nochmals auf 'OK' um den PC direkt neuzustarten.";
            if (cb1.SelectedIndex == 4)
                cb1.IsEnabled = false;
            // Abmelden
            if (desc.Text == "Sie werden in 7 Sekunden abgemeldet. Drücken sie auf 'Abbrechen' um den Vorgang abzubrechen oder nochmals auf 'OK' um den sich direkt abzumelden.")
                SignOutPC();
            if (cb1.SelectedIndex == 1)
                SetSignOutTimer();
            if (cb1.SelectedIndex == 1)
                desc.Text = "Sie werden in 7 Sekunden abgemeldet. Drücken sie auf 'Abbrechen' um den Vorgang abzubrechen oder nochmals auf 'OK' um den sich direkt abzumelden.";
            if (cb1.SelectedIndex == 1)
                cb1.IsEnabled = false;
            // Energie sparen
            if (cb1.SelectedIndex == 2)
                SetSuspendState(false, true, true);
            if (cb1.SelectedIndex == 2)
                cb1.IsEnabled = false;
            if (cb1.SelectedIndex == 2)
                Close();
            // Konto wechseln
            if (cb1.SelectedIndex == 0)
                LockWorkStation();
            if (cb1.SelectedIndex == 0)
                cb1.IsEnabled = false;
            if (cb1.SelectedIndex == 0)
                Close();



        }
    }
}

