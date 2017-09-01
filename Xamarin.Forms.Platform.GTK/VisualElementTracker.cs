using Gdk;
using Gtk;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.GTK.Extensions;

namespace Xamarin.Forms.Platform.GTK
{
    public class VisualElementTracker<TElement, TNativeElement> : IDisposable where TElement : VisualElement where TNativeElement : Widget
    {
        private bool _isDisposed;
        private TNativeElement _control;
        private TElement _element;
        private EventBox _container;
        private bool _invalidateArrangeNeeded;
        private bool _isPanning;
        private bool _wasPanGestureStartedSent;
        private double _initialPosX;
        private double _initialPosY;

        private readonly NotifyCollectionChangedEventHandler _collectionChangedHandler;

        public event EventHandler Updated;

        public VisualElementTracker()
        {
            _collectionChangedHandler = ModelGestureRecognizersOnCollectionChanged;
        }

        public EventBox Container
        {
            get { return _container; }
            set
            {
                if (_container == value)
                    return;

                if (_container != null)
                {
                    _container.ButtonPressEvent -= OnContainerButtonPressEvent;
                    _container.ButtonPressEvent -= OnContainerPanStartEvent;
                    _container.WidgetEvent -= OnContainerPanMoveEvent;
                    _container.ButtonReleaseEvent -= OnContainerPanEndEvent;
                }

                _container = value;

                UpdatingGestureRecognizers();

                UpdateNativeControl();
            }
        }

        public TNativeElement Control
        {
            get { return _control; }
            set
            {
                if (_control == value)
                    return;

                if (_control != null)
                {
                    _control.ButtonPressEvent -= OnControlButtonPressEvent;
                }

                _control = value;
                UpdateNativeControl();

                if (PreventGestureBubbling)
                {
                    UpdatingGestureRecognizers();
                }
            }
        }

        public bool PreventGestureBubbling { get; set; }

        public TElement Element
        {
            get { return _element; }
            set
            {
                if (_element == value)
                    return;

                if (_element != null)
                {
                    _element.BatchCommitted -= OnRedrawNeeded;
                    _element.PropertyChanged -= OnPropertyChanged;

                    var view = _element as View;
                    if (view != null)
                    {
                        var oldRecognizers = (ObservableCollection<IGestureRecognizer>)view.GestureRecognizers;
                        oldRecognizers.CollectionChanged -= _collectionChangedHandler;
                    }
                }

                _element = value;

                if (_element != null)
                {
                    _element.BatchCommitted += OnRedrawNeeded;
                    _element.PropertyChanged += OnPropertyChanged;

                    var view = _element as View;
                    if (view != null)
                    {
                        var newRecognizers = (ObservableCollection<IGestureRecognizer>)view.GestureRecognizers;
                        newRecognizers.CollectionChanged += _collectionChangedHandler;
                    }
                }

                UpdateNativeControl();
            }
        }

        protected virtual void UpdateNativeControl()
        {
            if (Element == null || Container == null)
                return;

            UpdateVisibility(Element, Container);
            UpdateOpacity(Element, Container);
            UpdateScaleAndRotation(Element, Container);
            UpdateInputTransparent(Element, Container);

            if (_invalidateArrangeNeeded)
            {
                MaybeInvalidate();
            }
            _invalidateArrangeNeeded = false;

            OnUpdated();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed)
                return;

            _isDisposed = true;

            if (!disposing)
                return;

            if (_container != null)
            {
                _container.ButtonPressEvent -= OnContainerButtonPressEvent;
                _container.ButtonPressEvent -= OnContainerPanStartEvent;
                _container.WidgetEvent -= OnContainerPanMoveEvent;
                _container.ButtonReleaseEvent -= OnContainerPanEndEvent;
            }

            if (_element != null)
            {
                _element.BatchCommitted -= OnRedrawNeeded;
                _element.PropertyChanged -= OnPropertyChanged;

                var view = _element as View;
                if (view != null)
                {
                    var oldRecognizers = (ObservableCollection<IGestureRecognizer>)view.GestureRecognizers;
                    oldRecognizers.CollectionChanged -= _collectionChangedHandler;
                }
            }

            if (_control != null)
            {
                _control.ButtonPressEvent -= OnControlButtonPressEvent;
            }

            Container.Dispose();
            Container = null;
        }

        protected virtual void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Element.Batched)
            {
                if (e.PropertyName == VisualElement.XProperty.PropertyName || 
                    e.PropertyName == VisualElement.YProperty.PropertyName || 
                    e.PropertyName == VisualElement.WidthProperty.PropertyName ||
                    e.PropertyName == VisualElement.HeightProperty.PropertyName)
                {
                    _invalidateArrangeNeeded = true;
                }
                return;
            }

            if (e.PropertyName == VisualElement.XProperty.PropertyName || 
                e.PropertyName == VisualElement.YProperty.PropertyName || 
                e.PropertyName == VisualElement.WidthProperty.PropertyName ||
                e.PropertyName == VisualElement.HeightProperty.PropertyName)
            {
                MaybeInvalidate();
            }
            else if (e.PropertyName == VisualElement.AnchorXProperty.PropertyName || 
                e.PropertyName == VisualElement.AnchorYProperty.PropertyName)
            {
                UpdateScaleAndRotation(Element, Container);
            }
            else if (e.PropertyName == VisualElement.ScaleProperty.PropertyName)
            {
                UpdateScaleAndRotation(Element, Container);
            }
            else if (e.PropertyName == VisualElement.TranslationXProperty.PropertyName || 
                e.PropertyName == VisualElement.TranslationYProperty.PropertyName ||
                     e.PropertyName == VisualElement.RotationProperty.PropertyName || 
                     e.PropertyName == VisualElement.RotationXProperty.PropertyName || 
                     e.PropertyName == VisualElement.RotationYProperty.PropertyName)
            {
                UpdateRotation(Element, Container);
            }
            else if (e.PropertyName == VisualElement.IsVisibleProperty.PropertyName)
            {
                UpdateVisibility(Element, Container);
            }
            else if (e.PropertyName == VisualElement.OpacityProperty.PropertyName)
            {
                UpdateOpacity(Element, Container);
            }
            else if (e.PropertyName == VisualElement.InputTransparentProperty.PropertyName)
            {
                UpdateInputTransparent(Element, Container);
            }
        }

        private void ModelGestureRecognizersOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            UpdatingGestureRecognizers();
        }

        private void OnUpdated()
        {
            Updated?.Invoke(this, EventArgs.Empty);
        }

        private void OnRedrawNeeded(object sender, EventArgs e)
        {
            UpdateNativeControl();
        }

        private void UpdatingGestureRecognizers()
        {
            var view = Element as View;
            IList<IGestureRecognizer> gestures = view?.GestureRecognizers;

            if (_container == null || gestures == null)
                return;

            _container.ButtonPressEvent -= OnContainerButtonPressEvent;
            _container.ButtonPressEvent -= OnContainerPanStartEvent;
            _container.WidgetEvent -= OnContainerPanMoveEvent;
            _container.ButtonReleaseEvent -= OnContainerPanEndEvent;

            if (gestures.GetGesturesFor<TapGestureRecognizer>(g => g.NumberOfTapsRequired == 1).Any())
            {
                _container.ButtonPressEvent += OnContainerButtonPressEvent;
            }
            else
            {
                if (_control != null && PreventGestureBubbling)
                {
                    _control.ButtonPressEvent += OnControlButtonPressEvent;
                }
            }

            bool hasPanGesture = gestures.GetGesturesFor<PanGestureRecognizer>().GetEnumerator().MoveNext();

            if (hasPanGesture)
            {
                _container.Events = EventMask.ButtonPressMask | EventMask.ButtonReleaseMask | EventMask.PointerMotionMask;
                _container.ButtonPressEvent += OnContainerPanStartEvent;
                _container.WidgetEvent += OnContainerPanMoveEvent;
                _container.ButtonReleaseEvent += OnContainerPanEndEvent;
            }
        }

        private void MaybeInvalidate()
        {
            if (Element.IsInNativeLayout)
                return;

            var parent = Container.Parent;
            parent?.QueueDraw();
            Container.QueueDraw();
        }

        // TODO: Implement Scale
        private static void UpdateScaleAndRotation(VisualElement view, EventBox eventBox)
        {
            double anchorX = view.AnchorX;
            double anchorY = view.AnchorY;
            double scale = view.Scale;

            UpdateRotation(view, eventBox);
        }

        // TODO: Implement Rotation
        private static void UpdateRotation(VisualElement view, EventBox eventBox)
        {
            if (view == null)
                return;

            double anchorX = view.AnchorX;
            double anchorY = view.AnchorY;
            double rotationX = view.RotationX;
            double rotationY = view.RotationY;
            double rotation = view.Rotation;
            double translationX = view.TranslationX;
            double translationY = view.TranslationY;
            double scale = view.Scale;

            var viewRenderer = Platform.GetRenderer(view) as Widget;

            if (viewRenderer == null)
                return;

            if (rotationX % 360 == 0 &&
                rotationY % 360 == 0 &&
                rotation % 360 == 0 &&
                translationX == 0 &&
                translationY == 0 &&
                scale == 1)
            {
                return;
            }
            else
            {
                viewRenderer.MoveTo(
                    scale == 0 ? 0 : translationX / scale,
                    scale == 0 ? 0 : translationY / scale);
            }
        }

        private static void UpdateVisibility(VisualElement view, EventBox eventBox)
        {
            eventBox.Visible = view.IsVisible;
        }

        // TODO: Implement Opacity
        private static void UpdateOpacity(VisualElement view, EventBox eventBox)
        {

        }

        // TODO: Implement InputTransparent
        private static void UpdateInputTransparent(VisualElement view, EventBox eventBox)
        {
    
        }

        private void OnContainerButtonPressEvent(object o, ButtonPressEventArgs args)
        {
            if (args.Event.Button != 1)
                return;

            var view = Element as View;

            if (view == null)
                return;

            if (args.Event.Type == Gdk.EventType.TwoButtonPress)
            {
                IEnumerable<TapGestureRecognizer> doubleTapGestures = view.GestureRecognizers
                    .GetGesturesFor<TapGestureRecognizer>(g => g.NumberOfTapsRequired == 2);

                foreach (TapGestureRecognizer recognizer in doubleTapGestures)
                    recognizer.SendTapped(view);
            }
            else
            {
                IEnumerable<TapGestureRecognizer> tapGestures = view.GestureRecognizers
                    .GetGesturesFor<TapGestureRecognizer>(g => g.NumberOfTapsRequired == 1);

                foreach (TapGestureRecognizer recognizer in tapGestures)
                    recognizer.SendTapped(view);
            }
        }

        private void OnContainerPanStartEvent(object o, ButtonPressEventArgs args)
        {
            if (args.Event.Button != 1)
                return;

            var view = Element as View;

            if (view == null)
                return;

            _initialPosX = args.Event.X;
            _initialPosY = args.Event.Y;

            _wasPanGestureStartedSent = false;
            _isPanning = true;
        }

        private void OnContainerPanMoveEvent(object o, WidgetEventArgs args)
        {
            if (!_isPanning)
                return;

            if (args.Event.Type == EventType.MotionNotify)
            {
                var eventMotion = args.Event as EventMotion;

                if (eventMotion != null)
                {
                    int translationX = 0;
                    int translationY = 0;
                    ModifierType state;

                    eventMotion.Window.GetPointer(out translationX, out translationY, out state);

                    if ((state & ModifierType.Button1Mask) == 0)
                        return;

                    var view = Element as View;

                    if (view == null)
                        return;

                    HandlePan(translationX - _initialPosX, translationY - _initialPosY, view);
                }
            }
        }

        private void OnContainerPanEndEvent(object o, ButtonReleaseEventArgs args)
        {
            var view = Element as View;

            if (view == null)
                return;

            PanComplete(true);
        }

        private void HandlePan(double translationX, double translationY, View view)
        {
            if (view == null)
                return;

            _isPanning = true;

            foreach (PanGestureRecognizer recognizer in view.GestureRecognizers.GetGesturesFor<PanGestureRecognizer>().Where(g => g.TouchPoints == 1))
            {
                if (!_wasPanGestureStartedSent)
                {
                    recognizer.SendPanStarted(view, Application.Current.PanGestureId);
                }
                recognizer.SendPan(view, translationX, translationY, Application.Current.PanGestureId);
            }

            _wasPanGestureStartedSent = true;
        }

        private void PanComplete(bool success)
        {
            var view = Element as View;

            if (view == null || !_isPanning)
                return;

            foreach (PanGestureRecognizer recognizer in view.GestureRecognizers.GetGesturesFor<PanGestureRecognizer>().Where(g => g.TouchPoints == 1))
            {
                if (success)
                {
                    recognizer.SendPanCompleted(view, Application.Current.PanGestureId);
                }
                else
                {
                    recognizer.SendPanCanceled(view, Application.Current.PanGestureId);
                }
            }

            Application.Current.PanGestureId++;
            _isPanning = false;
        }

        private void OnControlButtonPressEvent(object o, ButtonPressEventArgs args)
        {
            args.RetVal = true;
        }

        private void OnControlButtonReleaseEvent(object o, ButtonReleaseEventArgs args)
        {
            args.RetVal = true;
        }
    }
}