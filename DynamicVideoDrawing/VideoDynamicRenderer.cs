using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Input.StylusPlugIns;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DynamicVideoDrawing
{
    class VideoDynamicRenderer : DynamicRenderer
    {
        private readonly Uri _videoUri;
        private readonly Lazy<MediaPlayer> _player;
        private Point _firstPoint;
        private Point _lastPoint;

        public VideoDynamicRenderer(Uri videoUri)
        {
            _videoUri = videoUri;
            _player = new Lazy<MediaPlayer>(() =>
              {
                  var player = new MediaPlayer();
                  player.MediaOpened += Player_MediaOpened;
                  player.MediaEnded += Player_MediaEnded;
                  player.Open(_videoUri);
                  return player;
              });
        }

        private void Player_MediaOpened(object sender, EventArgs e)
        {
            _player.Value.Play();
        }

        private void Player_MediaEnded(object sender, EventArgs e)
        {
            _player.Value.Play();
        }

        protected override void OnStylusDown(RawStylusInput rawStylusInput)
        {
            _firstPoint = rawStylusInput.GetStylusPoints().First().ToPoint();
            _lastPoint = _firstPoint;
            base.OnStylusDown(rawStylusInput);
        }

        protected override void OnStylusMove(RawStylusInput rawStylusInput)
        {
            _lastPoint = rawStylusInput.GetStylusPoints().Last().ToPoint();
            base.OnStylusMove(rawStylusInput);
            Reset(null, new StylusPointCollection(new[] { _firstPoint, _lastPoint }));
        }

        protected override void OnDraw(DrawingContext drawingContext, StylusPointCollection stylusPoints,
            Geometry geometry, Brush fillBrush)
        {
            drawingContext.DrawVideo(_player.Value, new Rect(_firstPoint, _lastPoint));
        }
    }
}
