
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied

CREATE TABLE `player_arena` (
  `PlayerID` int(6) unsigned NOT NULL,
  `Rank` int(7) unsigned NOT NULL,
  `DeckCharacter1` int(8) unsigned NOT NULL,
  `DeckCharacter2` int(8) unsigned NOT NULL,
  `DeckCharacter3` int(8) unsigned NOT NULL,
  `DeckCharacter4` int(8) unsigned NOT NULL,
  `DeckCharacter5` int(8) unsigned NOT NULL,
  `Win` int(7) unsigned DEFAULT '0',
  `Lose` int(7) unsigned DEFAULT '0',
  `DefenseWin` int(7) unsigned DEFAULT '0',
  `DefenseLose` int(7) unsigned DEFAULT '0',
  `AllHP` int(10) unsigned DEFAULT '0',
  `AllStrength` int(10) unsigned DEFAULT '0',
  `PlayCount` int(10) unsigned DEFAULT '0',
  `ResetCount` int(10) unsigned DEFAULT '0',
  `CoolTime` text NOT NULL,
  `CreatedAt` text NOT NULL,
  `UpdatedAt` text NOT NULL,
  PRIMARY KEY (`PlayerID`),
  UNIQUE KEY `ID_UNIQUE` (`PlayerID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE TABLE `player_battle_session` (
  `PlayerID` int(10) unsigned NOT NULL,
  `SessionID` varchar(64) NOT NULL,
  `PreprocessData` text NOT NULL,
  `PlayerCharacterID1` int(10) unsigned NOT NULL,
  `PlayerCharacterID2` int(10) unsigned NOT NULL,
  `PlayerCharacterID3` int(10) unsigned NOT NULL,
  `PlayerCharacterID4` int(10) unsigned NOT NULL,
  `PlayerCharacterID5` int(10) unsigned NOT NULL,
  PRIMARY KEY (`PlayerID`),
  UNIQUE KEY `i1` (`SessionID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE TABLE `player_character` (
  `ID` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `PlayerID` int(10) unsigned NOT NULL,
  `CharacterID` int(10) unsigned NOT NULL,
  `Level` int(10) unsigned NOT NULL DEFAULT '1',
  `Exp` int(10) unsigned NOT NULL DEFAULT '0',
  `PromotionLevel` int(10) unsigned NOT NULL DEFAULT '1',
  `EvolutionLevel` int(10) unsigned NOT NULL DEFAULT '1',
  `ActionLevel1` int(10) unsigned NOT NULL DEFAULT '1',
  `ActionLevel2` int(10) unsigned NOT NULL DEFAULT '1',
  `ActionLevel3` int(10) unsigned NOT NULL DEFAULT '1',
  `ActionLevel4` int(10) unsigned NOT NULL DEFAULT '1',
  `ActionLevel5` int(10) unsigned NOT NULL DEFAULT '1',
  `ActionLevel6` int(10) unsigned NOT NULL DEFAULT '1',
  `Gear1` int(10) unsigned NOT NULL DEFAULT '0',
  `Gear2` int(10) unsigned NOT NULL DEFAULT '0',
  `Gear3` int(10) unsigned NOT NULL DEFAULT '0',
  `Gear4` int(10) unsigned NOT NULL DEFAULT '0',
  `Gear5` int(10) unsigned NOT NULL DEFAULT '0',
  `Gear6` int(10) unsigned NOT NULL DEFAULT '0',
  `CreatedAt` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedAt` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`ID`),
  UNIQUE KEY `PlayerCharacterID_UNIQUE` (`ID`)
) ENGINE=InnoDB AUTO_INCREMENT=883 DEFAULT CHARSET=latin1;
CREATE TABLE `player_cooperate_character` (
  `PlayerID` int(10) unsigned NOT NULL,
  `CooperateID` int(10) unsigned NOT NULL,
  `Level` int(10) unsigned NOT NULL,
  PRIMARY KEY (`PlayerID`,`CooperateID`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
CREATE TABLE `player_daily_quest` (
  `PlayerID` int(10) unsigned NOT NULL,
  `DailyQuestID` int(10) unsigned NOT NULL,
  `Current` int(10) unsigned NOT NULL,
  `ReceiveReward` tinyint(3) unsigned NOT NULL,
  `CreatedAt` int(10) unsigned NOT NULL,
  `UpdatedAt` int(10) unsigned NOT NULL,
  PRIMARY KEY (`PlayerID`,`DailyQuestID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE TABLE `player_episode` (
  `PlayerID` int(10) unsigned NOT NULL,
  `EpisodeID` int(10) unsigned NOT NULL,
  `CreatedAt` int(10) unsigned NOT NULL,
  `UpdatedAt` int(10) unsigned NOT NULL,
  PRIMARY KEY (`PlayerID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE TABLE `player_expedition` (
  `PlayerID` int(10) unsigned NOT NULL,
  `Level` tinyint(3) unsigned NOT NULL,
  `ResetNum` tinyint(3) unsigned NOT NULL,
  `ResetedAt` int(10) unsigned NOT NULL,
  `CreatedAt` int(10) unsigned NOT NULL,
  `UpdatedAt` int(10) unsigned NOT NULL,
  PRIMARY KEY (`PlayerID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE TABLE `player_expedition_session` (
  `PlayerID` int(10) unsigned NOT NULL,
  `ExpeditionID` int(10) unsigned NOT NULL,
  `ClearStep` tinyint(4) NOT NULL,
  `CanAcquirePrize` tinyint(4) NOT NULL,
  `PreprocessData` text NOT NULL,
  `PlayerCharacterData` text NOT NULL,
  `CreatedAt` int(10) unsigned NOT NULL,
  `UpdatedAt` int(10) unsigned NOT NULL,
  PRIMARY KEY (`PlayerID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE TABLE `player_item` (
  `PlayerID` int(10) unsigned NOT NULL,
  `ItemID` int(10) unsigned NOT NULL,
  `PossessionsCount` int(10) unsigned NOT NULL DEFAULT '0',
  `CreatedAt` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedAt` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`PlayerID`,`ItemID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE TABLE `player_login_bonus` (
  `PlayerID` int(10) unsigned NOT NULL,
  `LoginBonusID` int(10) unsigned NOT NULL,
  `CurrentNum` tinyint(3) unsigned NOT NULL,
  `ReceiveRewardThreshold` tinyint(3) unsigned NOT NULL,
  `LastLoggedInAt` int(10) unsigned NOT NULL,
  `CreatedAt` int(10) unsigned NOT NULL,
  `UpdatedAt` int(10) unsigned NOT NULL,
  PRIMARY KEY (`PlayerID`,`LoginBonusID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE TABLE `player_quest_trigger` (
  `PlayerID` int(10) unsigned NOT NULL,
  `TriggerID` int(10) unsigned NOT NULL,
  `CurrentNum` int(10) unsigned NOT NULL,
  `ReceiveRewardThreshold` int(10) unsigned NOT NULL,
  `CreatedAt` int(10) unsigned NOT NULL,
  `UpdatedAt` int(10) unsigned NOT NULL,
  PRIMARY KEY (`PlayerID`,`TriggerID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE TABLE `player_shop` (
  `PlayerID` int(10) unsigned NOT NULL,
  `ShopID` int(10) unsigned NOT NULL,
  `MerchandiseIDs` text NOT NULL,
  `BoughtMerchandiseIDs` text NOT NULL,
  `MerchandiseManualUpdateNum` smallint(5) unsigned NOT NULL,
  `MerchandiseUpdatedAtManually` int(10) unsigned NOT NULL,
  `MerchandiseUpdatedAtOnSchedule` int(10) unsigned NOT NULL,
  `CreatedAt` int(10) unsigned NOT NULL,
  `UpdatedAt` int(10) unsigned NOT NULL,
  PRIMARY KEY (`PlayerID`,`ShopID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE TABLE `player_stage` (
  `PlayerID` int(10) unsigned NOT NULL,
  `StageID` int(10) unsigned NOT NULL,
  `Progress` int(1) unsigned NOT NULL DEFAULT '0',
  `CreatedAt` int(10) unsigned NOT NULL,
  `UpdatedAt` int(10) unsigned NOT NULL,
  `PlayCount` int(10) unsigned NOT NULL DEFAULT '0',
  `ResetCount` int(10) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`PlayerID`,`StageID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE TABLE `player_status` (
  `ID` int(10) unsigned NOT NULL,
  `Language` int(10) unsigned NOT NULL,
  `Name` varchar(64) DEFAULT NULL,
  `Exp` int(10) unsigned DEFAULT '0',
  `VipExp` int(10) unsigned DEFAULT '0',
  `GuildID` int(10) unsigned DEFAULT '0',
  `StaminaTime` int(10) unsigned NOT NULL,
  `StaminaOverCount` int(11) unsigned DEFAULT '0',
  `Gold` int(10) unsigned DEFAULT '0',
  `FreeGem` int(10) unsigned DEFAULT '0',
  `ChargeGem` int(10) unsigned DEFAULT '0',
  `AvaterCharacterID` int(10) unsigned DEFAULT '11',
  `CreatedAt` int(10) unsigned NOT NULL,
  `UpdatedAt` int(10) unsigned NOT NULL,
  PRIMARY KEY (`ID`),
  UNIQUE KEY `PlayerID_UNIQUE` (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
CREATE TABLE `player_trials` (
  `PlayerID` int(10) unsigned NOT NULL,
  `TrialsID` int(10) unsigned NOT NULL,
  `PlayCount` int(10) unsigned NOT NULL,
  `PlayMaxCount` int(10) unsigned NOT NULL,
  `CoolTime` int(10) unsigned NOT NULL,
  `ResetAt` int(10) unsigned NOT NULL,
  `CreatedAt` int(10) unsigned NOT NULL,
  `UpdatedAt` int(10) unsigned NOT NULL,
  PRIMARY KEY (`PlayerID`,`TrialsID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE TABLE `player_trials_level` (
  `PlayerID` int(10) unsigned NOT NULL,
  `TrialsLevelID` int(10) unsigned NOT NULL,
  `Rank` int(1) unsigned DEFAULT '0',
  `CreatedAt` int(10) unsigned NOT NULL,
  `UpdatedAt` int(10) unsigned NOT NULL,
  PRIMARY KEY (`PlayerID`,`TrialsLevelID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back

DROP TABLE `player_arena`;
DROP TABLE `player_battle_session`;
DROP TABLE `player_character`;
DROP TABLE `player_cooperate_character`;
DROP TABLE `player_daily_quest`;
DROP TABLE `player_episode`;
DROP TABLE `player_expedition`;
DROP TABLE `player_expedition_session`;
DROP TABLE `player_item`;
DROP TABLE `player_login_bonus`;
DROP TABLE `player_quest_trigger`;
DROP TABLE `player_shop`;
DROP TABLE `player_stage`;
DROP TABLE `player_status`;
DROP TABLE `player_trials`;
DROP TABLE `player_trials_level`;

