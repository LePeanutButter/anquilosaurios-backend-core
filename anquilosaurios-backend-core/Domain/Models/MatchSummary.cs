using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace aquilosaurios_backend_core.Domain.Models
{
    #region Round Summary

    /// <summary>
    /// Represents a summary of a single round within a match.
    /// Tracks winners and deaths per player.
    /// </summary>
    public class RoundSummary
    {
        #region Properties

        /// <summary>
        /// The sequential number of the round within the match.
        /// </summary>
        [BsonElement("roundNumber")]
        public int RoundNumber { get; set; }

        /// <summary>
        /// List of player IDs who won this round.
        /// </summary>
        [BsonElement("winnersIds")]
        public List<Guid> WinnersIds { get; set; } = [];

        /// <summary>
        /// A mapping of player IDs to the number of deaths they had during this round.
        /// </summary>
        [BsonElement("deathsPerPlayer")]
        public Dictionary<Guid, int> DeathsPerPlayer { get; set; } = [];

        #endregion
    }

    #endregion

    #region Match Summary

    /// <summary>
    /// Represents a summary of a multiplayer match in the game.
    /// Stores participants, winners, rounds, and timestamps.
    /// </summary>
    public class MatchSummary
    {
        #region Properties 

        /// <summary>
        /// Unique identifier for the match.
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// List of player IDs who participated in the match.
        /// </summary>
        [BsonElement("playersIds")]
        public List<Guid> PlayersIds { get; set; } = [];

        /// <summary>
        /// List of player IDs who won the match.
        /// </summary>
        [BsonElement("winnersIds")]
        public List<Guid> WinnersIds { get; set; } = [];

        /// <summary>
        /// List of round summaries for this match.
        /// </summary>
        [BsonElement("rounds")]
        public List<RoundSummary> Rounds { get; set; } = [];

        /// <summary>
        /// Timestamp indicating when the match started.
        /// </summary>
        [BsonElement("startedAt")]
        public DateTime StartedAt { get; set; }

        /// <summary>
        /// Timestamp indicating when the match ended.
        /// </summary>
        [BsonElement("endedAt")]
        public DateTime EndedAt { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Adds a round summary to the match.
        /// </summary>
        /// <param name="round">The round summary to add.</param>
        public void AddRound(RoundSummary round)
        {
            Rounds ??= [];
            Rounds.Add(round);
        }

        /// <summary>
        /// Adds a player to the match.
        /// </summary>
        /// <param name="playerId">Player's GUID.</param>
        public void AddPlayer(Guid playerId)
        {
            PlayersIds ??= [];
            if (!PlayersIds.Contains(playerId))
                PlayersIds.Add(playerId);
        }

        /// <summary>
        /// Adds a winner to the match.
        /// </summary>
        /// <param name="winnerId">Winner's GUID.</param>
        public void AddWinner(Guid winnerId)
        {
            WinnersIds ??= [];
            if (!WinnersIds.Contains(winnerId))
                WinnersIds.Add(winnerId);
        }

        #endregion
    }

    #endregion
}
