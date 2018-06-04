﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Hillinworks.TiledImage.Properties;
using Hillinworks.TiledImage.Utilities;

namespace Hillinworks.TiledImage.Controls
{
	partial class TiledImageView
	{
		/// <summary>
		/// The ghost tiledImage is an tiledImage captured before any LOD change, scaled and painted as a placeholder
		/// while the tiles under the new LOD are being loaded. It helps creating a smoother zooming experience.
		/// </summary>
		private ImageSource GhostImage { get; set; }
		private Rect GhostImageWorldRect { get; set; }

		[SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
		private void CaptureGhostImage()
		{
			if (DesignerProperties.GetIsInDesignMode(this))
			{
				return;
			}

			if (this.ActualWidth == 0 || this.ActualHeight == 0)
			{
				return;
			}

			var renderTarget = new RenderTargetBitmap((int)this.ActualWidth, (int)this.ActualHeight, 96, 96, PixelFormats.Pbgra32);
			renderTarget.Render(this);
			renderTarget.Freeze();

#if DEBUG
			if (Features.SaveGhostImage)
			{
				renderTarget.SaveAsPng(Features.GhostImageSavePath);
			}
#endif

			this.GhostImage = renderTarget;
			this.GhostImageWorldRect = new Rect(
				this.ViewState.ViewToWorldMatrix.Transform(new Point()),
				new Size(
					this.ActualWidth * this.ViewState.ViewToWorldScale,
					this.ActualHeight * this.ViewState.ViewToWorldScale));
		}


		private void RenderGhostImage(DrawingContext context)
		{
			if (this.GhostImage == null)
			{
				return;
			}

			context.DrawImage(this.GhostImage, this.GhostImageWorldRect);
		}
	}
}
