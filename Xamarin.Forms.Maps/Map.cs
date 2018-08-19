using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Xamarin.Forms.Maps
{
	public class Map : View, IEnumerable<Pin>
	{
		public static readonly BindableProperty PinsSourceProperty = BindableProperty.Create(nameof(PinsSource), typeof(ObservableCollection<Pin>), typeof(Map), default(ObservableCollection<Pin>), propertyChanged: (bindable, oldvalue, newvalue) =>
		{
			if (newvalue != null)
			{
				var map = (Map)bindable;
				var pins = (ObservableCollection<Pin>)newvalue;

				foreach (var pin in pins)
					map.Pins.Add(pin);

				pins.CollectionChanged += (sender, e) =>
				{
					Device.BeginInvokeOnMainThread(() =>
					{
						switch (e.Action)
						{
							case NotifyCollectionChangedAction.Add:
							case NotifyCollectionChangedAction.Replace:
							case NotifyCollectionChangedAction.Remove:
								if (e.OldItems != null)
									foreach (var item in e.OldItems)
										map.Pins.Remove((Pin)item);
								if (e.NewItems != null)
									foreach (var item in e.NewItems)
										map.Pins.Add((Pin)item);
								break;
							case NotifyCollectionChangedAction.Reset:
								map.Pins.Clear();
								break;
						}
					});
				};
			}
		});

		public static readonly BindableProperty PinTemplateProperty = BindableProperty.Create(nameof(PinTemplate), typeof(DataTemplate), typeof(Map));

		public static readonly BindableProperty MapTypeProperty = BindableProperty.Create("MapType", typeof(MapType), typeof(Map), default(MapType));

		public static readonly BindableProperty IsShowingUserProperty = BindableProperty.Create("IsShowingUser", typeof(bool), typeof(Map), default(bool));

		public static readonly BindableProperty HasScrollEnabledProperty = BindableProperty.Create("HasScrollEnabled", typeof(bool), typeof(Map), true);

		public static readonly BindableProperty HasZoomEnabledProperty = BindableProperty.Create("HasZoomEnabled", typeof(bool), typeof(Map), true);

		readonly ObservableCollection<Pin> _pins = new ObservableCollection<Pin>();
		MapSpan _visibleRegion;

		public Map(MapSpan region)
		{
			LastMoveToRegion = region;

			VerticalOptions = HorizontalOptions = LayoutOptions.FillAndExpand;

			_pins.CollectionChanged += PinsOnCollectionChanged;
		}

		// center on Rome by default
		public Map() : this(new MapSpan(new Position(41.890202, 12.492049), 0.1, 0.1))
		{
		}

		public ObservableCollection<Pin> PinsSource
		{
			get { return (ObservableCollection<Pin>)GetValue(PinsSourceProperty); }
			set { SetValue(PinsSourceProperty, value); }
		}

		public DataTemplate PinTemplate
		{
			get { return (DataTemplate)GetValue(PinTemplateProperty); }
			set { SetValue(PinTemplateProperty, value); }
		}

		public bool HasScrollEnabled
		{
			get { return (bool)GetValue(HasScrollEnabledProperty); }
			set { SetValue(HasScrollEnabledProperty, value); }
		}

		public bool HasZoomEnabled
		{
			get { return (bool)GetValue(HasZoomEnabledProperty); }
			set { SetValue(HasZoomEnabledProperty, value); }
		}

		public bool IsShowingUser
		{
			get { return (bool)GetValue(IsShowingUserProperty); }
			set { SetValue(IsShowingUserProperty, value); }
		}

		public MapType MapType
		{
			get { return (MapType)GetValue(MapTypeProperty); }
			set { SetValue(MapTypeProperty, value); }
		}

		public IList<Pin> Pins
		{
			get { return _pins; }
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetVisibleRegion(MapSpan value) => VisibleRegion = value;
		public MapSpan VisibleRegion
		{
			get { return _visibleRegion; }
			internal set
			{
				if (_visibleRegion == value)
					return;
				if (value == null)
					throw new ArgumentNullException(nameof(value));
				OnPropertyChanging();
				_visibleRegion = value;
				OnPropertyChanged();
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public MapSpan LastMoveToRegion { get; private set; }

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public IEnumerator<Pin> GetEnumerator()
		{
			return _pins.GetEnumerator();
		}

		public void MoveToRegion(MapSpan mapSpan)
		{
			if (mapSpan == null)
				throw new ArgumentNullException(nameof(mapSpan));
			LastMoveToRegion = mapSpan;
			MessagingCenter.Send(this, "MapMoveToRegion", mapSpan);
		}

		void PinsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.NewItems != null && e.NewItems.Cast<Pin>().Any(pin => pin.Label == null))
				throw new ArgumentException("Pin must have a Label to be added to a map");
		}
	}
}