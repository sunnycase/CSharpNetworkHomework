using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace DynamicVideoDrawing
{
    class VideoInkCanvas : InkCanvas
    {
        public static readonly DependencyProperty VideoSourceProperty = DependencyProperty.Register(nameof(VideoSource),
            typeof(Uri), typeof(VideoInkCanvas), new PropertyMetadata(OnVideoSourcePropertyChanged));

        public Uri VideoSource
        {
            get { return (Uri)GetValue(VideoSourceProperty); }
            set { SetValue(VideoSourceProperty, value); }
        }

        private static void OnVideoSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as VideoInkCanvas)?.OnVideoSourceChanged();
        }

        private void OnVideoSourceChanged()
        {
            var uri = VideoSource;
            if (uri != null)
                this.DynamicRenderer = new VideoDynamicRenderer(uri);
            else
                this.DynamicRenderer = null;
        }

        protected override void OnStrokeCollected(InkCanvasStrokeCollectedEventArgs e)
        {
            Strokes.Remove(e.Stroke);
            var stroke = new VideoStroke(e.Stroke.StylusPoints, VideoSource);
            Strokes.Add(stroke);
            base.OnStrokeCollected(new InkCanvasStrokeCollectedEventArgs(stroke));
        }
    }
}
