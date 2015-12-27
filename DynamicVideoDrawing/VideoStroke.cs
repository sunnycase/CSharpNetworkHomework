using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;

namespace DynamicVideoDrawing
{
    class VideoStroke : Stroke
    {
        private readonly Lazy<MediaPlayer> _player;
        public VideoStroke(StylusPointCollection stylusPoints, Uri videoUri)
            :base(stylusPoints)
        {
            _player = new Lazy<MediaPlayer>(() =>
            {
                var player = new MediaPlayer();
                player.MediaOpened += Player_MediaOpened;
                player.MediaEnded += Player_MediaEnded;
                player.Open(videoUri);
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

        protected override void DrawCore(DrawingContext drawingContext, DrawingAttributes drawingAttributes)
        {
            var firstPoint = StylusPoints.First().ToPoint();
            var lastPoint = StylusPoints.Last().ToPoint();
            drawingContext.DrawVideo(_player.Value, new Rect(firstPoint, lastPoint));
        }
    }
}
