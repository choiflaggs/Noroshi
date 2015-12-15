
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied
ALTER TABLE `player_arena`
 MODIFY `BattleStartedAt` int unsigned NOT NULL AFTER `PlayNum`,
 CHANGE `ResetNum` `PlayResetNum` int unsigned NOT NULL,
 ADD `LastBattledAt` int unsigned NOT NULL AFTER `BattleStartedAt`,
 ADD `LastPlayResetAt` int unsigned NOT NULL AFTER `PlayResetNum`,
 ADD `CoolTimeResetNum` int unsigned NOT NULL AFTER `CoolTimeAt`,
 ADD `LastCoolTimeResetAt` int unsigned NOT NULL AFTER `CoolTimeResetNum`;


-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back
ALTER TABLE `player_arena`
 CHANGE `PlayResetNum` `ResetNum` int unsigned NOT NULL,
 MODIFY `BattleStartedAt` int unsigned NOT NULL AFTER `ResetNum`,
 DROP `LastCoolTimeResetAt`,
 DROP `CoolTimeResetNum`,
 DROP `LastPlayResetAt`,
 DROP `LastBattledAt`;

