/*create table if not exists `file`
(
    `id` int primary key auto_increment,
    `url` text null,
    `task_id` varchar(36) null,
    `type` int not null,
    `completed_setting_id` int not null,
    `created_date` datetime(3) not null
)charset=utf8mb4;

create table if not exists `completed_setting`
(
    `id` int primary key auto_increment,
    `file_path` varchar(255) not null,
    `upload_address_type` int not null,
    `created_date` datetime(3) not null
)charset=utf8mb4;

create table if not exists `file_task`
(
    `id` varchar(36) primary key,
    `status` int not null,
    `created_date` datetime(3) not null
)charset=utf8mb4;*/