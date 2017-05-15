﻿using System;
using Gtk;
using Pango;

namespace Xamarin.Forms.Platform.GTK.Controls
{
    public class SearchEntry : Gtk.Frame
    {
        private HBox _container;
        private EntryWrapper _entryWrapper;
        private ImageButton _searchButton;
        private ImageButton _clearButton;
        private bool _isEnabled;

        public SearchEntry()
        {
            _container = new HBox();
            _entryWrapper = new EntryWrapper();
            _entryWrapper.Entry.HasFrame = false;
            _searchButton = new ImageButton();
            _searchButton.SetImagePosition(PositionType.Left);
            _searchButton.ImageWidget.Pixbuf = RenderIcon("gtk-find", IconSize.SmallToolbar, null);

            _clearButton = new ImageButton();
            _clearButton.SetImagePosition(PositionType.Left);
            _clearButton.ImageWidget.Pixbuf = RenderIcon("gtk-close", IconSize.SmallToolbar, null);

            _container.PackStart(_searchButton, false, false, 0);
            _container.PackStart(_entryWrapper);

            _entryWrapper.Entry.Changed += EntryChanged;
            _clearButton.Clicked += CancelButtonClicked;

            Add(_container);
        }

        public string SearchText
        {
            get
            {
                return _entryWrapper.Entry.Text;
            }
            set
            {
                _entryWrapper.Entry.Text = value;
            }
        }


        public string PlaceholderText
        {
            get
            {
                return _entryWrapper.PlaceholderText;
            }
            set
            {
                _entryWrapper.PlaceholderText = value;
            }
        }

        public bool IsEnabled
        {
            get
            {
                return _isEnabled;
            }

            set
            {
                _isEnabled = value;
                UpdateIsEnabled();
            }
        }

        public event EventHandler SearchTextChanged
        {
            add
            {
                var entry = _entryWrapper?.Entry;

                if (entry != null)
                {
                    entry.Changed += value;
                }

            }

            remove
            {
                var entry = _entryWrapper?.Entry;

                if (entry != null)
                {
                    entry.Changed -= value;
                }
            }
        }

        public event EventHandler SearchButtonClicked
        {
            add
            {
                if (_searchButton != null)
                {
                    _searchButton.Clicked += value;
                }
            }

            remove
            {
                if (_searchButton != null)
                {
                    _searchButton.Clicked -= value;
                }
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            if (_entryWrapper?.Entry != null)
            {
                _entryWrapper.Entry.Changed -= EntryChanged;
            }

            if (_clearButton != null)
            {
                _clearButton.Clicked -= CancelButtonClicked;
            }
        }

        public void SetBackgroundColor(Gdk.Color color)
        {
            _searchButton.SetBackgroundColor(color);
            _entryWrapper.SetBackgroundColor(color);
        }

        public void SetTextColor(Gdk.Color color)
        {
            _entryWrapper.SetTextColor(color);
        }

        public void SetPlaceholderTextColor(Gdk.Color color)
        {
            _entryWrapper.SetPlaceholderTextColor(color);
        }

        public void SetCancelButtonColor(Gdk.Color color)
        {
            _clearButton.SetBackgroundColor(color);
        }

        public void SetFont(FontDescription fontDescription)
        {
            _entryWrapper.SetFont(fontDescription);
        }

        public void SetAlignment(float alignmentValue)
        {
            _entryWrapper.SetAlignment(alignmentValue);
        }

        private void ShowClearButton()
        {
            if (_clearButton.Parent == null)
            {
                _container.PackEnd(_clearButton, false, false, 0);
                _clearButton.ShowAll();
            }
        }

        private void RemoveClearButton()
        {
            if (_clearButton.Parent != null)
            {
                _container.Remove(_clearButton);
            }
        }

        private void UpdateIsEnabled()
        {
            _entryWrapper.IsEnabled = _isEnabled;
            _searchButton.Sensitive = _isEnabled;
            _clearButton.Sensitive = _isEnabled;
        }

        private void EntryChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_entryWrapper.Entry.Text))
            {
                ShowClearButton();
            }
            else
            {
                RemoveClearButton();
            }
        }

        private void CancelButtonClicked(object sender, EventArgs e)
        {
            _entryWrapper.Entry.Text = string.Empty;
        }
    }
}