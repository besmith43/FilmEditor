using System;
using Xunit;
using FilmEditor.Commands;
using FilmEditor.Data;

namespace FilmEditor.Test
{
    public class TestScaleCommand
    {
        [Fact]
        public void TestGetAspectRatio_16x9_1080p()
        {
            ScaleCommand scaleCommand = new (new string[] {});

            string result = scaleCommand.GetAspectRatio((int)Scale._1080width16x9, (int)Scale._1080height16x9);

            Assert.Equal("16x9", result);
        }

        [Fact]
        public void TestGetAspectRatio_16x9_720p()
        {
            ScaleCommand scaleCommand = new (new string[] {});

            string result = scaleCommand.GetAspectRatio((int)Scale._720width16x9, (int)Scale._720height16x9);

            Assert.Equal("16x9", result);
        }

        [Fact]
        public void TestGetAspectRatio_16x9_480p()
        {
            ScaleCommand scaleCommand = new (new string[] {});

            string result = scaleCommand.GetAspectRatio((int)Scale._480width16x9, (int)Scale._480height16x9);

            Assert.Equal("16x9", result);
        }

        [Fact]
        public void TestGetAspectRatio_16x9_2k()
        {
            ScaleCommand scaleCommand = new (new string[] {});

            string result = scaleCommand.GetAspectRatio((int)Scale._2kwidth16x9, (int)Scale._2kheight16x9);

            Assert.Equal("16x9", result);
        }

        [Fact]
        public void TestGetAspectRatio_16x9_4k()
        {
            ScaleCommand scaleCommand = new (new string[] {});

            string result = scaleCommand.GetAspectRatio((int)Scale._4kwidth16x9, (int)Scale._4kheight16x9);

            Assert.Equal("16x9", result);
        }

        [Fact]
        public void TestGetAspectRatio_4x3_1080p()
        {
            ScaleCommand scaleCommand = new (new string[] {});

            string result = scaleCommand.GetAspectRatio((int)Scale._1080width4x3, (int)Scale._1080height4x3);

            Assert.Equal("4x3", result);
        }

        [Fact]
        public void TestGetAspectRatio_4x3_720p()
        {
            ScaleCommand scaleCommand = new (new string[] {});

            string result = scaleCommand.GetAspectRatio((int)Scale._720width4x3, (int)Scale._720height4x3);

            Assert.Equal("4x3", result);
        }

        [Fact]
        public void TestGetAspectRatio_4x3_480p()
        {
            ScaleCommand scaleCommand = new (new string[] {});

            string result = scaleCommand.GetAspectRatio((int)Scale._480width4x3, (int)Scale._480height4x3);

            Assert.Equal("4x3", result);
        }

        [Fact]
        public void TestGetAspectRatio_4x3_2k()
        {
            ScaleCommand scaleCommand = new (new string[] {});

            string result = scaleCommand.GetAspectRatio((int)Scale._2kwidth4x3, (int)Scale._2kheight4x3);

            Assert.Equal("4x3", result);
        }

        [Fact]
        public void TestGetAspectRatio_4x3_4k()
        {
            ScaleCommand scaleCommand = new (new string[] {});

            string result = scaleCommand.GetAspectRatio((int)Scale._4kwidth4x3, (int)Scale._4kheight4x3);

            Assert.Equal("4x3", result);
        }

        [Fact]
        public void TestGetAspectRatio_4x3_720x540()
        {
            ScaleCommand scaleCommand = new (new string[] {});

            string result = scaleCommand.GetAspectRatio(720, 540);

            Assert.Equal("4x3", result);
        }

        [Fact]
        public void TestGetAspectRatio_Custom_720x404()
        {
            ScaleCommand scaleCommand = new (new string[] {});

            string result = scaleCommand.GetAspectRatio(720, 404);

            Assert.Equal("Aspect Ratio not found", result);
        }

        [Fact]
        public void TestGetAspectRatio_Custom_1920x796()
        {
            ScaleCommand scaleCommand = new (new string[] {});

            string result = scaleCommand.GetAspectRatio(1920, 796);

            Assert.Equal("Aspect Ratio not found", result);
        }
    }
}
