using aquilosaurios_backend_core.Domain.Models;
using FluentAssertions;
using Xunit;

namespace aquilosaurios_backend_core.Tests.Domain.Tests.Models
{
    public class RoundSummaryTests
    {
        [Fact]
        public void RoundSummary_ShouldInitializeProperties()
        {
            var round = new RoundSummary();

            round.RoundNumber.Should().Be(0);
            round.WinnersIds.Should().NotBeNull();
            round.WinnersIds.Should().BeEmpty();
            round.DeathsPerPlayer.Should().NotBeNull();
            round.DeathsPerPlayer.Should().BeEmpty();
        }

        [Fact]
        public void Can_AddWinnersAndDeaths()
        {
            var round = new RoundSummary();
            var playerId = Guid.NewGuid();

            round.WinnersIds.Add(playerId);
            round.DeathsPerPlayer[playerId] = 2;

            round.WinnersIds.Should().Contain(playerId);
            round.DeathsPerPlayer[playerId].Should().Be(2);
        }
    }

    public class MatchSummaryTests
    {
        [Fact]
        public void MatchSummary_ShouldInitializeProperties()
        {
            var match = new MatchSummary();

            match.Id.Should().NotBe(Guid.Empty);
            match.PlayersIds.Should().NotBeNull().And.BeEmpty();
            match.WinnersIds.Should().NotBeNull().And.BeEmpty();
            match.Rounds.Should().NotBeNull().And.BeEmpty();
            match.StartedAt.Should().Be(default);
            match.EndedAt.Should().Be(default);
        }

        [Fact]
        public void AddPlayer_ShouldAddUniquePlayers()
        {
            var match = new MatchSummary();
            var playerId = Guid.NewGuid();

            match.AddPlayer(playerId);
            match.AddPlayer(playerId);

            match.PlayersIds.Should().ContainSingle().And.Contain(playerId);
        }

        [Fact]
        public void AddWinner_ShouldAddUniqueWinners()
        {
            var match = new MatchSummary();
            var winnerId = Guid.NewGuid();

            match.AddWinner(winnerId);
            match.AddWinner(winnerId);

            match.WinnersIds.Should().ContainSingle().And.Contain(winnerId);
        }

        [Fact]
        public void AddRound_ShouldAddRounds()
        {
            var match = new MatchSummary();
            var round = new RoundSummary { RoundNumber = 1 };

            match.AddRound(round);

            match.Rounds.Should().ContainSingle();
            match.Rounds[0].RoundNumber.Should().Be(1);
        }

        [Fact]
        public void AddMultipleRoundsAndPlayers_ShouldWorkCorrectly()
        {
            var match = new MatchSummary();
            var round1 = new RoundSummary { RoundNumber = 1 };
            var round2 = new RoundSummary { RoundNumber = 2 };
            var player1 = Guid.NewGuid();
            var player2 = Guid.NewGuid();

            match.AddRound(round1);
            match.AddRound(round2);
            match.AddPlayer(player1);
            match.AddPlayer(player2);
            match.AddWinner(player1);

            match.Rounds.Should().HaveCount(2);
            match.PlayersIds.Should().HaveCount(2);
            match.WinnersIds.Should().HaveCount(1).And.Contain(player1);
        }

        [Fact]
        public void RoundSummary_ShouldInitializeEmptyCollections_WhenCreated()
        {
            var round = new RoundSummary();

            round.WinnersIds.Should().NotBeNull().And.BeEmpty();
            round.DeathsPerPlayer.Should().NotBeNull().And.BeEmpty();
        }

        [Fact]
        public void Can_AddWinnersAndDeaths_WhenCollectionsAreEmpty()
        {
            var round = new RoundSummary();
            var playerId = Guid.NewGuid();

            round.WinnersIds.Add(playerId);
            round.DeathsPerPlayer[playerId] = 2;

            round.WinnersIds.Should().Contain(playerId);
            round.DeathsPerPlayer[playerId].Should().Be(2);
        }

        [Fact]
        public void MatchSummary_ShouldInitializeEmptyCollections_WhenCreated()
        {
            var match = new MatchSummary();

            match.PlayersIds.Should().NotBeNull().And.BeEmpty();
            match.WinnersIds.Should().NotBeNull().And.BeEmpty();
            match.Rounds.Should().NotBeNull().And.BeEmpty();
        }

        [Fact]
        public void AddPlayer_ShouldInitializePlayersList_WhenEmpty()
        {
            var match = new MatchSummary();
            var playerId = Guid.NewGuid();

            match.AddPlayer(playerId);

            match.PlayersIds.Should().Contain(playerId);
        }

        [Fact]
        public void AddWinner_ShouldInitializeWinnersList_WhenEmpty()
        {
            var match = new MatchSummary();
            var winnerId = Guid.NewGuid();

            match.AddWinner(winnerId);

            match.WinnersIds.Should().Contain(winnerId);
        }

        [Fact]
        public void AddRound_ShouldInitializeRoundsList_WhenEmpty()
        {
            var match = new MatchSummary();
            var round = new RoundSummary { RoundNumber = 1 };

            match.AddRound(round);

            match.Rounds.Should().Contain(round);
        }

    }
}
