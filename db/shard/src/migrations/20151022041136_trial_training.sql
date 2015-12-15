
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied
ALTER TABLE `player_trials` 
CHANGE COLUMN `TrialsID` `TrialID` INT(10) UNSIGNED NOT NULL , RENAME TO  `player_trial` ;

ALTER TABLE `player_trials_level` 
CHANGE COLUMN `TrialsLevelID` `TrialStageID` INT(10) UNSIGNED NOT NULL ,
CHANGE COLUMN `Rank` `Rank` TINYINT UNSIGNED NOT NULL , RENAME TO  `player_trial_stage` ;

ALTER TABLE `player_episode` 
RENAME TO  `player_story_episode` ;

ALTER TABLE `player_stage` 
CHANGE COLUMN `PlayCount` `PlayCount` INT(10) UNSIGNED NOT NULL AFTER `Rank`,
CHANGE COLUMN `MaxPlayCount` `MaxPlayCount` INT(10) UNSIGNED NOT NULL AFTER `PlayCount`,
CHANGE COLUMN `Progress` `Rank` TINYINT(3) UNSIGNED NOT NULL , RENAME TO `player_story_stage` ;

CREATE TABLE `player_training_stage` (
  `PlayerID` INT UNSIGNED NOT NULL,
  `TrainingStageID` INT UNSIGNED NOT NULL,
  `Rank` TINYINT UNSIGNED NOT NULL,
  `CreatedAt` INT UNSIGNED NOT NULL,
  `UpdatedAt` INT UNSIGNED NOT NULL,
  PRIMARY KEY (`PlayerID`, `TrainingStageID`));

CREATE TABLE `player_training` (
  `PlayerID` INT UNSIGNED NOT NULL,
  `TrainingID` INT UNSIGNED NOT NULL,
  `PlayCount` INT UNSIGNED NOT NULL,
  `PlayMaxCount` INT UNSIGNED NOT NULL,
  `CoolTime` INT UNSIGNED NOT NULL,
  `ResetAt` INT UNSIGNED NOT NULL,
  `CreatedAt` INT UNSIGNED NOT NULL,
  `UpdatedAt` INT UNSIGNED NOT NULL,
  PRIMARY KEY (`PlayerID`, `TrainingID`));


-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back
ALTER TABLE `player_trial` 
CHANGE COLUMN `TrialID` `TrialsID` INT(10) UNSIGNED NOT NULL , RENAME TO  `player_trials` ;

ALTER TABLE `player_trial_stage` 
CHANGE COLUMN `TrialStageID` `TrialsLevelID` INT(10) UNSIGNED NOT NULL ,
CHANGE COLUMN `Rank` `Rank` INT(1) UNSIGNED NULL DEFAULT '0' , RENAME TO  `player_trial_stage` ;

ALTER TABLE `player_story_episode` 
RENAME TO  `player_episode` ;

ALTER TABLE `player_story_stage` 
CHANGE COLUMN `PlayCount` `PlayCount` INT(10) UNSIGNED NOT NULL AFTER `UpdatedAt`,
CHANGE COLUMN `MaxPlayCount` `MaxPlayCount` INT(10) UNSIGNED NOT NULL AFTER `PlayCount`,
CHANGE COLUMN `Rank` `Progress` TINYINT(3) UNSIGNED NOT NULL , RENAME TO `player_stage` ;

DROP TABLE `player_training` ;

DROP TABLE `player_training_stage`;