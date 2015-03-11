﻿namespace DotNetThumbor
{
    using System;
    using System.Collections.Generic;

    public class ThumborImage : IThumborImage
    {
        private readonly string thumborSecretKey;

        private readonly Uri thumborServerUrl;

        private Uri imageUrl;

        private bool smartImage;

        private Thumbor.ImageFormat outputFormat;

        private Thumbor.ImageHorizontalAlign horizontalAlign;

        private Thumbor.ImageVerticalAlign verticalAlign;

        private string cropCoordinates;

        private int? quality;

        private bool grayscale;

        private List<string> watermarks = new List<string>();

        private string fillColour;

        private bool trim;

        private string fitin;

        private bool flipImageHorizonal;

        private bool flipImageVertical;

        private int? width;

        private int? height;

        private int? brightness;

        private int? contrast;

        private string colorize;

        private bool equalize;

        private int? maxBytes;

        public ThumborImage(Uri thumborServerUrl, string thumborSecretKey, string imageUrl)
        {
            try
            {
                this.imageUrl = new Uri(imageUrl);
            }
            catch (UriFormatException ex)
            {
                throw new ArgumentException("Invalid URL", ex);
            }

            this.thumborSecretKey = thumborSecretKey;
            this.thumborServerUrl = thumborServerUrl;
        }

        public IThumborImage Resize(int? newWidth, int? newHeight)
        {
            this.width = newWidth ?? 0;
            this.height = newHeight ?? 0;

            return this;
        }

        public IThumborImage Smart(bool doSmartImage)
        {
            this.smartImage = doSmartImage;
            return this;
        }

        public IThumborImage Format(Thumbor.ImageFormat imageFormat)
        {
            this.outputFormat = imageFormat;
            return this;
        }

        public IThumborImage Crop(int topLeft, int topRight, int bottomLeft, int bottomRight)
        {
            this.cropCoordinates = string.Format("{0}x{1}:{2}x{3}", topLeft, topRight, bottomLeft, bottomRight);
            return this;
        }

        public IThumborImage Quality(int? imageQuality)
        {
            this.quality = imageQuality;
            return this;
        }

        public IThumborImage Grayscale(bool grayscaleImage)
        {
            this.grayscale = grayscaleImage;
            return this;
        }

        public IThumborImage Watermark(string watermarkImageUrl, int right, int down, int transparency)
        {
            this.watermarks.Add(string.Format("watermark({0},{1},{2},{3})", watermarkImageUrl, right, down, transparency));
            return this;
        }

        public IThumborImage Fill(string fillInColour)
        {
            this.fillColour = fillInColour;
            return this;
        }

        public IThumborImage Trim(bool trimImage)
        {
            this.trim = trimImage;
            return this;
        }

        public IThumborImage FitIn(bool fitIn)
        {
            this.fitin = fitIn ? "fit-in" : string.Empty;
            return this;
        }

        public IThumborImage FullFitIn(bool fullFitIn)
        {
            this.fitin = fullFitIn ? "full-fit-in" : string.Empty;
            return this;
        }

        public IThumborImage HorizontalAlign(Thumbor.ImageHorizontalAlign align)
        {
            this.horizontalAlign = align;
            return this;
        }

        public IThumborImage VerticalAlign(Thumbor.ImageVerticalAlign align)
        {
            this.verticalAlign = align;
            return this;
        }

        public IThumborImage HorizontalFlip(bool flipHorizontal)
        {
            this.width = this.width ?? 0;
            this.flipImageHorizonal = flipHorizontal;
            return this;
        }

        public IThumborImage VerticalFlip(bool flipVertical)
        {
            this.height = this.height ?? 0;
            this.flipImageVertical = flipVertical;
            return this;
        }

        public override string ToString()
        {
            return this.ToUrl();
        }

        public string ToUnsafeUrl()
        {
            var server = this.thumborServerUrl + "unsafe/";
            return server + this.FormatUrlParts();
        }

        public IThumborImage Brightness(int imageBrightness)
        {
            this.brightness = imageBrightness;
            return this;
        }

        public IThumborImage Contrast(int imageContrast)
        {
            this.contrast = imageContrast;
            return this;
        }

        public IThumborImage Colorize(int redPercentage, int greenPercentage, int bluePercentage, string fillColor)
        {
            this.colorize = string.Format("colorize({0},{1},{2},{3})", redPercentage, greenPercentage, bluePercentage, fillColor);
            return this;
        }

        public IThumborImage Equalize(bool equalizeImage)
        {
            this.equalize = equalizeImage;
            return this;
        }

        public IThumborImage MaxBytes(int? imageMaxBytes)
        {
            this.maxBytes = imageMaxBytes;
            return this;
        }

        public string ToUrl()
        {
            if (this.imageUrl == null)
            {
                throw new InvalidOperationException("BuildImage must be called before ToUrl");
            }

            if (string.IsNullOrEmpty(this.thumborSecretKey))
            {
                return this.ToUnsafeUrl();
            }

            var urlparts = this.FormatUrlParts();
            var server = this.thumborServerUrl + ThumborSigner.Encode(urlparts, this.thumborSecretKey) + "/";

            return server + urlparts;
        }

        private string FormatUrlParts()
        {
            var urlParts = new List<string>();

            if (this.trim)
            {
                urlParts.Add("trim");
            }

            if (!string.IsNullOrEmpty(this.cropCoordinates))
            {
                urlParts.Add(this.cropCoordinates);
            }

            if (!string.IsNullOrEmpty(this.fitin))
            {
                urlParts.Add(this.fitin);
            }

            if (this.width != null || this.height != null)
            {
                string widthString;
                if (this.width == 0 && this.flipImageHorizonal)
                {
                    widthString = "-0";
                }
                else if (this.flipImageHorizonal)
                {
                    this.width = this.width * -1;
                    widthString = this.width.ToString();
                }
                else
                {
                    widthString = this.width.ToString();
                }

                string heightString;
                if (this.height == 0 && this.flipImageVertical)
                {
                    heightString = "-0";
                }
                else if (this.flipImageVertical)
                {
                    this.height = this.height * -1;
                    heightString = this.height.ToString();
                }
                else
                {
                    heightString = this.height.ToString();
                }

                urlParts.Add(widthString + "x" + heightString);
            }

            if (this.horizontalAlign != Thumbor.ImageHorizontalAlign.Center)
            {
                urlParts.Add(this.horizontalAlign.ToString().ToLower());
            }

            if (this.verticalAlign != Thumbor.ImageVerticalAlign.Middle)
            {
                urlParts.Add(this.verticalAlign.ToString().ToLower());
            }

            if (this.smartImage)
            {
                urlParts.Add("smart");
            }

            var filters = new List<string>();
            if (this.outputFormat != Thumbor.ImageFormat.None)
            {
                filters.Add(string.Format("format({0})", this.outputFormat.ToString().ToLower()));
            }

            if (this.quality != null)
            {
                filters.Add(string.Format("quality({0})", this.quality));
            }

            if (this.grayscale)
            {
                filters.Add("grayscale()");
            }

            if (this.watermarks.Count != 0)
            {
                filters.AddRange(this.watermarks);
            }

            if (!string.IsNullOrEmpty(this.fillColour))
            {
                filters.Add(string.Format("fill({0})", this.fillColour));
            }

            if (this.brightness != null)
            {
                filters.Add(string.Format("brightness({0})", this.brightness));
            }

            if (this.contrast != null)
            {
                filters.Add(string.Format("contrast({0})", this.contrast));
            }

            if (!string.IsNullOrEmpty(this.colorize))
            {
                filters.Add(this.colorize);
            }

            if (this.equalize)
            {
                filters.Add("equalize()");
            }

            if (this.maxBytes != null)
            {
                filters.Add(string.Format("max_bytes({0})", this.maxBytes));
            }

            if (filters.Count != 0)
            {
                urlParts.Add("filters:" + string.Join(":", filters));
            }

            urlParts.Add(this.imageUrl.ToString());

            return string.Join("/", urlParts);
        }
    }
}
