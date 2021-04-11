using System;
using Xunit;
using FilmEditor.Commands;

namespace FilmEditor.Test
{
    public class TestRenameCommand
    {
        [Fact]
        public void TestGetEpisodeFilename_seasonSingle_episodeSingle()
        {
            RenameCommand renameCommand = new(new string[] {});

            renameCommand.showName = "TestTVName";

            string episodeName = renameCommand.getEpisodeFilename("2", "1");

            Assert.Equal("TestTVName s01e02.mp4", episodeName);
        }

        [Fact]
        public void TestGetEpisodeFilename_seasonDouble_episodeSingle()
        {
            RenameCommand renameCommand = new(new string[] {});

            renameCommand.showName = "TestTVName";

            string episodeName = renameCommand.getEpisodeFilename("2", "12");

            Assert.Equal("TestTVName s12e02.mp4", episodeName);
        }

        [Fact]
        public void TestGetEpisodeFilename_seasonSingle_episodeDouble()
        {
            RenameCommand renameCommand = new(new string[] {});

            renameCommand.showName = "TestTVName";

            string episodeName = renameCommand.getEpisodeFilename("12", "1");

            Assert.Equal("TestTVName s01e12.mp4", episodeName);
        }

        [Fact]
        public void TestGetEpisodeFilename_seasonDouble_episodeDouble()
        {
            RenameCommand renameCommand = new(new string[] {});

            renameCommand.showName = "TestTVName";

            string episodeName = renameCommand.getEpisodeFilename("23", "12");

            Assert.Equal("TestTVName s12e23.mp4", episodeName);
        }
    }
}
