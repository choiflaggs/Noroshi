
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied

DROP TABLE `player_daily_quest`;
CREATE TABLE `player_daily_quest_trigger` (
  `PlayerID` int unsigned NOT NULL,
  `TriggerID` int unsigned NOT NULL,
  `CurrentNum` int unsigned NOT NULL,
  `ReceiveRewardThreshold` int unsigned NOT NULL,
  `CreatedAt` int unsigned NOT NULL,
  `UpdatedAt` int unsigned NOT NULL,
  PRIMARY KEY (`PlayerID`,`TriggerID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back

DROP TABLE `player_daily_quest_trigger`;
CREATE TABLE `player_daily_quest` (
  `PlayerID` int(10) unsigned NOT NULL,
  `DailyQuestID` int(10) unsigned NOT NULL,
  `Current` int(10) unsigned NOT NULL,
  `ReceiveReward` tinyint(3) unsigned NOT NULL,
  `CreatedAt` int(10) unsigned NOT NULL,
  `UpdatedAt` int(10) unsigned NOT NULL,
  PRIMARY KEY (`PlayerID`,`DailyQuestID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
