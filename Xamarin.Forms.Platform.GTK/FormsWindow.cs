using Gtk;
using System;
using System.ComponentModel;
using System.Threading;
using Xamarin.Forms.Platform.GTK.Extensions;

namespace Xamarin.Forms.Platform.GTK
{
    public class FormsWindow : Window
    {
        private Application _application;
        private MenuBar _menuBar;
        private Gtk.Menu _menu;
        private Gdk.Size _lastSize;

        public FormsWindow()
            : base(WindowType.Toplevel)
        {
            SetDefaultSize(800, 600);
            SetSizeRequest(400, 400);

            MainThreadID = Thread.CurrentThread.ManagedThreadId;
            MainWindow = this;

            if (SynchronizationContext.Current == null)
                SynchronizationContext.SetSynchronizationContext(new GtkSynchronizationContext());
        }

        public static int MainThreadID { get; set; }
        public static Window MainWindow { get; set; }

        public void LoadApplication(Application application)
        {
            if (application == null)
                throw new ArgumentNullException(nameof(application));

            Application.SetCurrentApplication(application);
            _application = application;

            application.PropertyChanged += ApplicationOnPropertyChanged;
            UpdateMainPage();
        }

        public void SetApplicationTitle(string title)
        {
            if (string.IsNullOrEmpty(title))
                return;

            Title = title;
        }

        public void SetApplicationIcon(string icon)
        {
            if (string.IsNullOrEmpty(icon))
                return;

            var appliccationIconPixbuf = new Gdk.Pixbuf(icon);
            Icon = appliccationIconPixbuf;
        }

        public sealed override void Dispose()
        {
            base.Dispose();

            Dispose(true);
        }

        protected override bool OnDeleteEvent(Gdk.Event evnt)
        {
            base.OnDeleteEvent(evnt);

            Gtk.Application.Quit();

            return true;
        }

        private void ApplicationOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(Application.MainPage))
                UpdateMainPage();
            if (args.PropertyName == nameof(Menu))
                UpdateMainPage();
        }

        protected override bool OnConfigureEvent(Gdk.EventConfigure evnt)
        {
            Gdk.Size newSize = new Gdk.Size(evnt.Width, evnt.Height);

            if (_lastSize != newSize)
            {
                _lastSize = newSize;
                var pageRenderer = Platform.GetRenderer(_application.MainPage);
                pageRenderer?.SetElementSize(new Size(newSize.Width, newSize.Height));
            }

            return base.OnConfigureEvent(evnt);
        }

        private void UpdateMainPage()
        {
            if (_application.MainPage == null)
                return;

            var platformRenderer = Child as PlatformRenderer;

            if (platformRenderer != null)
            {
                RemoveChildIfExists();
                ((IDisposable)platformRenderer.Platform).Dispose();
            }

            var platform = new Platform();
            platform.PlatformRenderer.SetSizeRequest(WidthRequest, HeightRequest);

            _menuBar = new MenuBar();
            _menu = new Gtk.Menu();

            VBox vbox = new VBox(false, 0);
            vbox.PackStart(_menuBar, false, false, 0);
            vbox.PackStart(platform.PlatformRenderer, true, true, 0);
            Add(vbox);
            
            platform.SetPage(_application.MainPage);

            var mainMenu = Element.GetMenu(_application);

            if (mainMenu != null)
                SetMainMenu(mainMenu);

            Child.ShowAll();
        }

        private void RemoveChildIfExists()
        {
            foreach (var child in Children)
            {
                var widget = child as Widget;

                if (widget != null)
                {
                    Remove(widget);
                }
            }
        }

        private void Dispose(bool disposing)
        {
            if (disposing && _application != null)
            {
                _application.PropertyChanged -= ApplicationOnPropertyChanged;
            }
        }

        private void SetMainMenu(Menu mainMenu)
        {
            mainMenu.PropertyChanged += MainMenuOnPropertyChanged;
            MainMenuOnPropertyChanged(this, null);
        }

        private void MainMenuOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // TODO:
            Element.GetMenu(_application).ToGtkMenu();
        }
    }
}