SET @@DEFAULT_STORAGE_ENGINE = 'InnoDB';
SET @@AUTO_INCREMENT_OFFSET = 1;
SET @@AUTO_INCREMENT_INCREMENT = 1;
SET @@UNIQUE_CHECKS=0;
SET @@FOREIGN_KEY_CHECKS=0;
SET @@SQL_MODE='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';

DROP SCHEMA IF EXISTS `app`;
CREATE SCHEMA `app` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
USE `app`;

CREATE TABLE `app`.`user` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `surname` VARCHAR(45) NOT NULL,
  `first_name` VARCHAR(45) NOT NULL,
  `email` VARCHAR(45) NOT NULL,
  `password` VARCHAR(45) NOT NULL,
  PRIMARY KEY (`id`));

CREATE TABLE `app`.`client` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `user_id` INT NOT NULL,
  `connection_id` VARCHAR(45) NOT NULL,
  `admin_password` VARCHAR(45) NULL DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `connection_id_UNIQUE` (`connection_id` ASC) VISIBLE,
  INDEX `fk_client_user1_idx` (`user_id` ASC) VISIBLE,
  CONSTRAINT `fk_client_user1`
    FOREIGN KEY (`user_id`)
    REFERENCES `app`.`user` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);

CREATE TABLE `app`.`account` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `client_id` INT NOT NULL,
  `balance` DECIMAL(10,2) NOT NULL,
  PRIMARY KEY (`id`),
  INDEX `fk_account_client1_idx` (`client_id` ASC) VISIBLE,
  CONSTRAINT `fk_account_client1`
    FOREIGN KEY (`client_id`)
    REFERENCES `app`.`client` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);

CREATE TABLE `app`.`trader` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `user_id` INT NOT NULL,
  `connection_id` VARCHAR(45) NOT NULL,
  `label` VARCHAR(500) NOT NULL,
  `address` VARCHAR(500) NOT NULL,
  `phone` VARCHAR(45) NULL DEFAULT NULL,
  `logo_path` VARCHAR(45) NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `connection_id_UNIQUE` (`connection_id` ASC) VISIBLE,
  INDEX `fk_trader_user1_idx` (`user_id` ASC) VISIBLE,
  CONSTRAINT `fk_trader_user1`
    FOREIGN KEY (`user_id`)
    REFERENCES `app`.`user` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);

CREATE TABLE `app`.`commercial_link` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `trader_id` INT NULL,
  `client_id` INT NULL,
  `type` INT NOT NULL,
  PRIMARY KEY (`id`),
  INDEX `fk_commercial_link_trader1_idx` (`trader_id` ASC) VISIBLE,
  INDEX `fk_commercial_link_client1_idx` (`client_id` ASC) VISIBLE,
  CONSTRAINT `fk_commercial_link_trader1`
    FOREIGN KEY (`trader_id`)
    REFERENCES `app`.`trader` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_commercial_link_client1`
    FOREIGN KEY (`client_id`)
    REFERENCES `app`.`client` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);

CREATE TABLE `app`.`comment` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `commercial_link_id` INT NOT NULL,
  `creation_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `text` VARCHAR(500) NULL DEFAULT NULL,
  `rating` INT NULL DEFAULT NULL,
  PRIMARY KEY (`id`),
  INDEX `fk_comment_commercial_link1_idx` (`commercial_link_id` ASC) VISIBLE,
  CONSTRAINT `fk_comment_commercial_link1`
    FOREIGN KEY (`commercial_link_id`)
    REFERENCES `app`.`commercial_link` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);

CREATE TABLE `app`.`offer` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `trader_id` INT NULL,
  `start_time` DATETIME NOT NULL,
  `end_time` DATETIME NOT NULL,
  `content_path` VARCHAR(200) NOT NULL,
  PRIMARY KEY (`id`),
  INDEX `fk_offer_trader1_idx` (`trader_id` ASC) VISIBLE,
  CONSTRAINT `fk_offer_trader1`
    FOREIGN KEY (`trader_id`)
    REFERENCES `app`.`trader` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);

CREATE TABLE `app`.`client_offer` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `client_id` INT NULL,
  `offer_id` INT NOT NULL,
  `usesCount` INT NOT NULL,
  `sentCount` INT NOT NULL,
  PRIMARY KEY (`id`),
  INDEX `fk_client_offer_client1_idx` (`client_id` ASC) VISIBLE,
  INDEX `fk_client_offer_offer1_idx` (`offer_id` ASC) VISIBLE,
  CONSTRAINT `fk_client_offer_client1`
    FOREIGN KEY (`client_id`)
    REFERENCES `app`.`client` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_client_offer_offer1`
    FOREIGN KEY (`offer_id`)
    REFERENCES `app`.`offer` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);

CREATE TABLE `app`.`purchase` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `commercial_link_id` INT NOT NULL,
  `paying_time` DATETIME NOT NULL,
  `amount` DECIMAL(10,2) NOT NULL,
  PRIMARY KEY (`id`),
  INDEX `fk_purchase_commercial_link1_idx` (`commercial_link_id` ASC) VISIBLE,
  CONSTRAINT `fk_purchase_commercial_link1`
    FOREIGN KEY (`commercial_link_id`)
    REFERENCES `app`.`commercial_link` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);
