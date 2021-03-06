﻿using System;
using System.Drawing;

namespace TileIconifier.Controls.IconListView
{
    class IconListViewItem : IDisposable
    {
        public IconListViewItem()
        {

        }

        public IconListViewItem(Image img)
        {
            Image = img;
        }

        /// <summary>
        ///     Listview in which the item is contained. Null if the item is not in a list view.
        /// </summary>
        internal IconListView ListView { get; set; }

        /// <summary>
        ///     Gets the bounds of this item relative to the client rectangle of its parent.
        /// </summary>
        public Rectangle Bounds
        {
            get
            {
                var bounds = Rectangle.Empty;

                if (ListView?.ItemDisplayBounds != null)
                {
                    bounds = ListView.ItemDisplayBounds[Index];
                    bounds.X += ListView.DisplayRectangle.X;
                    bounds.Y += ListView.DisplayRectangle.Y;
                }

                return bounds;
            }
        }

        public Rectangle ImageBounds
        {
            get
            {
                var imgBounds = Rectangle.Empty;

                if (Image != null && !Bounds.IsEmpty)
                {
                    imgBounds = Bounds;
                    //Some padding. Instead, maybe we could add an ImageSize property to the listview?
                    imgBounds.Inflate(-5, -5);
                    imgBounds = FitRectangle(imgBounds, Image.Size);
                }

                return imgBounds;
            }
        }

        public int Index => ListView?.Items.IndexOf(this) ?? -1;

        private string _text;
        public string Text
        {
            get { return _text; }
            set
            {
                if (_text != value)
                {
                    _text = value;
                    ListView?.Invalidate(Bounds);
                }
            }
        }

        private Image _image;
        public Image Image
        {
            get { return _image; }
            set
            {
                if (_image != value)
                {
                    _image = (Image)value.Clone();
                    ListView?.Invalidate(Bounds);
                }
            }
        }

        public bool Selected
        {
            get
            {
                return ListView?.SelectedIndex == Index;
            }
            set
            {
                if (ListView == null)
                {
                    throw new InvalidOperationException("Cannot select an item that is not owned by any IconListView.");
                }
                ListView.SelectedIndex = value ? Index : -1;
            }
        }

        public bool MouseIsOver => ListView != null && ListView.HotItemIndex == Index;

        /// <summary>
        ///     Returns the biggest rectangle that can fit in the specified rectangle
        ///     while respecting the aspect ratio of the specified size.
        /// </summary>
        /// <param name="containerRect"></param>
        /// <param name="aspectRatio"></param>
        /// <returns></returns>
        private static Rectangle FitRectangle(Rectangle containerRect, Size aspectRatio)
        {
            var scaleX = (float)containerRect.Width / aspectRatio.Width;
            var scaleY = (float)containerRect.Height / aspectRatio.Height;

            var scaleMin = Math.Min(scaleX, scaleY);

            var r = new RectangleF
            {
                Width = aspectRatio.Width*scaleMin,
                Height = aspectRatio.Height*scaleMin
            };
            r.X = containerRect.X + (containerRect.Width - r.Width) / 2;
            r.Y = containerRect.Y + (containerRect.Height - r.Height) / 2;

            return Rectangle.Round(r);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~IconListViewItem()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Image?.Dispose();
            }
        }
    }
}
