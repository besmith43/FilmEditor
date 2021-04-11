using System;
using Xunit;
using FilmEditor.Commands;

namespace FilmEditor.Test
{
    public class TestNewCommand
    {
        [Fact]
        public void TestConvertTime_60minutes()
        {
            NewCommand newCommand = new(new string[] {});

            int total = newCommand.convertTime("1.0");

            Assert.Equal(60, total);
        }

        public void TestConvertTime_hour_9minutes()
        {
            NewCommand newCommand = new(new string[] {});

            int total = newCommand.convertTime("1.9.0");

            Assert.Equal(69, total);
        }

        public void TestConvertTime_2hours_75minutes_15seconds()
        {
            NewCommand newCommand = new(new string[] {});

            int total = newCommand.convertTime("2.75.15");

            Assert.Equal(11715, total);
        }


        public void TestConvertTime_5seconds()
        {
            NewCommand newCommand = new(new string[] {});

            int total = newCommand.convertTime("5");

            Assert.Equal(5, total);
        }

        [Fact]
        public void TestGetEpisodeFilename_seasonSingle_episodeSingle()
        {
            NewCommand newCommand = new(new string[] {});

            newCommand.seasonNum = "1";
            newCommand.showName = "TestTVName";

            string episodeName = newCommand.getEpisodeFilename("2");

            Assert.Equal("TestTVName s01e02.mp4", episodeName);
        }

        [Fact]
        public void TestGetEpisodeFilename_seasonDouble_episodeSingle()
        {
            NewCommand newCommand = new(new string[] {});

            newCommand.seasonNum = "12";
            newCommand.showName = "TestTVName";

            string episodeName = newCommand.getEpisodeFilename("2");

            Assert.Equal("TestTVName s12e02.mp4", episodeName);
        }

        [Fact]
        public void TestGetEpisodeFilename_seasonSingle_episodeDouble()
        {
            NewCommand newCommand = new(new string[] {});

            newCommand.seasonNum = "1";
            newCommand.showName = "TestTVName";

            string episodeName = newCommand.getEpisodeFilename("12");

            Assert.Equal("TestTVName s01e12.mp4", episodeName);
        }

        [Fact]
        public void TestGetEpisodeFilename_seasonDouble_episodeDouble()
        {
            NewCommand newCommand = new(new string[] {});

            newCommand.seasonNum = "12";
            newCommand.showName = "TestTVName";

            string episodeName = newCommand.getEpisodeFilename("23");

            Assert.Equal("TestTVName s12e23.mp4", episodeName);
        }
    }
}
