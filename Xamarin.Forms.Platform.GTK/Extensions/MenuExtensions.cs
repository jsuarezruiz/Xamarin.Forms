namespace Xamarin.Forms.Platform.GTK.Extensions
{
    internal static class MenuExtensions
    {
        public static Gtk.Menu ToGtkMenu(this Menu menus, Gtk.Menu gtkMenu = null)
        {
            if (gtkMenu == null)
            {
                gtkMenu = new Gtk.Menu();
                gtkMenu.Title = menus.Text ?? string.Empty;
            }

            foreach (var menu in menus)
            {
                var menuItem = new Gtk.MenuItem(menu.Text ?? string.Empty);
                var subMenu = new Gtk.Menu();
                menuItem.Submenu = subMenu;

                foreach (var item in menu.Items)
                {
                    var subMenuItem = item.ToGtkMenuItem();
                    subMenu.Append(subMenuItem);

                    item.PropertyChanged += (sender, e) => (sender as MenuItem)?
                    .UpdateGtkMenuItem(subMenuItem, new string[] { e.PropertyName });
                }

                gtkMenu.Append(menuItem);
                menu.ToGtkMenu(subMenu);
            }

            return gtkMenu;
        }

        public static Gtk.MenuItem ToGtkMenuItem(this MenuItem menuItem, int i = -1)
        {
            var gtkMenuItem = new Gtk.MenuItem(menuItem.Text ?? string.Empty);

            gtkMenuItem.Sensitive = menuItem.IsEnabled;
            gtkMenuItem.Activated += (sender, e) => menuItem.Activate();

            return gtkMenuItem;
        }

        public static void UpdateGtkMenuItem(this MenuItem item, Gtk.MenuItem menuItem, string[] properties)
        {
            foreach (var property in properties)
            {
                if (property.Equals(nameof(MenuItem.Text)))
                {
                    menuItem.Name = item.Text;
                }
                if (property.Equals(nameof(MenuItem.IsEnabled)))
                {
                    menuItem.Sensitive = item.IsEnabled;
                }
            }
        }
    }
}