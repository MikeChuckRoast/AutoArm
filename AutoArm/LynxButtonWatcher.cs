namespace AutoArm
{
    public enum ButtonAnalysis
    {
        OK,
        RED,
        NOT_GREY,
    }

    public class LynxButtonWatcher
    {
        Rectangle region = Rectangle.Empty;

        public LynxButtonWatcher(Rectangle region)
        {
            FindButton(region);
        }

        public void FindButton(Rectangle region)
        {
            Point virtualOrigin = SystemInformation.VirtualScreen.Location;

            // Create a bitmap to hold the screenshot
            using (Bitmap bmp = new Bitmap(region.Width, region.Height))
            {
                // Capture the screen area
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    Point sourcePoint = new Point(
                        region.X + virtualOrigin.X,
                        region.Y + virtualOrigin.Y
                    ); // Adjust for virtual screen origin
                    g.CopyFromScreen(sourcePoint, Point.Empty, region.Size);
                }

                // Find a row of 33 pixels which are all 227
                bool foundButton = false;
                int buttonX = 0;
                int buttonY = 0;
                for (int iRow = 0; iRow < region.Height; iRow++)
                {
                    bool lastPixelMatch = false;
                    int pixelCount = 0;

                    for (int iCol = 0; iCol < region.Width; iCol++)
                    {
                        Color pixel = bmp.GetPixel(iCol, iRow);
                        if (pixel.R == 227 && pixel.G == 227 && pixel.B == 227)
                        {
                            if (lastPixelMatch)
                            {
                                pixelCount++;
                            }
                            else
                            {
                                lastPixelMatch = true;
                                pixelCount = 1;
                            }
                        }

                        if (pixelCount == 33)
                        {
                            foundButton = true;
                            buttonX = iCol - 32;
                            buttonY = iRow;
                            break;
                        }
                    }

                    if (foundButton)
                        break;
                }

                if (foundButton)
                {
                    // Create a new rectangle for the button
                    this.region = new Rectangle(
                        region.X + buttonX,
                        region.Y + buttonY,
                        34,
                        34
                    );
                }
                else
                {
                    // Throw an error
                    throw new Exception(
                        "Could not find button in the specified region."
                    );
                }

            }

        }

        public (ButtonAnalysis, Bitmap) AnalyzeScreen()
        {
            // Create a bitmap to hold the screenshot
            Bitmap bmp = new Bitmap(region.Width, region.Height);

            // Capture the screen area
            using (Graphics g = Graphics.FromImage(bmp))
            {
                Point virtualOrigin = SystemInformation.VirtualScreen.Location;
                Point sourcePoint = new Point(
                    region.X + virtualOrigin.X,
                    region.Y + virtualOrigin.Y
                ); // Adjust for virtual screen origin
                g.CopyFromScreen(sourcePoint, Point.Empty, region.Size);
            }

            // Check for any red pixels in the region
            // If capture is armed, there should be no red pixels at all
            // If capture is not armed, the red circle/slash should be visible
            bool hasRedPixel = false;
            bool mostlyGreyPixel = true;
            for (int i = 0; i < region.Width; i++)
            {
                for (int j = 0; j < region.Height; j++)
                {
                    Color pixel = bmp.GetPixel(i, j);
                    // Check for red pixels
                    if (pixel.R > 200 && pixel.G < 100 && pixel.B < 100)
                    {
                        hasRedPixel = true;
                    }
                    // Check for grey pixels
                    if (
                        Math.Abs(pixel.R - pixel.G) > 10
                        || Math.Abs(pixel.R - pixel.B) > 10
                        || Math.Abs(pixel.G - pixel.B) > 10
                    )
                    {
                        // If any pixel is not grey, return false
                        mostlyGreyPixel = false;
                    }
                }
            }

            // For debugging: Save the screenshot to a file
            //bmp.Save("capture.png");

            // Determine the result based on pixel analysis
            if (hasRedPixel)
                return (ButtonAnalysis.RED, bmp);
            else if (!mostlyGreyPixel)
                return (ButtonAnalysis.NOT_GREY, bmp);
            else
                return (ButtonAnalysis.OK, bmp);
        }
    }
}
