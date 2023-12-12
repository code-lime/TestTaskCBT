<br />
<div align="center">
  <h3 align="center">TestTaskCBT</h3>

  <p align="center">
    Тестовое задание для "Центр банковских технологий"
    <br />
  </p>
</div>



<details>
  <summary>Оглавление</summary>
  <ol>
    <li>
      <a href="#about-the-project">About The Project</a>
    </li>
    <li>
      <a href="#getting-started">Getting Started</a>
      <ul>
        <li><a href="#prerequisites">Prerequisites</a></li>
        <li><a href="#installation">Installation</a></li>
      </ul>
    </li>
    <li><a href="#usage">Usage</a></li>
  </ol>
</details>



## About The Project

WEB приложение - планировщик мероприятий

## Getting Started

Описание шагов для запуска программы 

### Prerequisites

Действия для установки `docker-compose` на Ubuntu
* Требуется для установки и выполнения последующих шагов
  ```sh
  sudo apt-get install curl
  sudo apt-get install gnupg
  sudo apt-get install ca-certificates
  sudo apt-get install lsb-release
  ```
* Загрузка файла `gpg` для установки `docker`
  ```sh
  sudo mkdir -p /etc/apt/keyrings
  curl -fsSL https://download.docker.com/linux/ubuntu/gpg | sudo gpg --dearmor -o /etc/apt/keyrings/docker.gpg
  ```
* Добавление пакетов `docker` и `docker-compose` в Ubuntu
  ```
  echo "deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.gpg] https://download.docker.com/sudo apt-get install docker-ce docker-ce-cli containerd.io docker-compose-pluginsudo apt-get install docker-ce docker-ce-cli containerd.io docker-compose-pluginlinux/ubuntu   $(lsb_release -cs) stable" | sudo tee /etc/apt/sources.list.d/docker.list > /dev/null
  ```
* Установка `docker` и `docker-compose`
  ```
  sudo apt-get update
  sudo apt-get install docker-ce docker-ce-cli containerd.io docker-compose-plugin
  ```
* Проверка успешности установки `docker`
  ```
  sudo docker run hello-world
  ```

### Installation

* Клонирование репозитория
   ```sh
   git clone https://github.com/code-lime/TestTaskCBT
   ```
* Настройка параметров запуска
   ```sh
   cp .env.sample .env
   nano .env
   ```
   > Параметры запуска:
   > ```cs
   > MARIADB_DATABASE=... //Название базы данных
   > MARIADB_USERNAME=... //Пользователь базы данных
   > MARIADB_PASSWORD=... //Пароль от пользователя базы данных
   > MARIADB_PORT=... //Порт базы данных
   >
   > LOGGING_LEVEL=... //Уровни логирования (Verbose, Debug, Information, Warning, Error, Fatal) 
   >
   > JWT_ISSUER=... //Параметр JWT токена авторизации
   > JWT_AUDIENCE=... //Параметр JWT токена авторизации
   > JWT_KEY=... //Параметр JWT токена авторизации (требуемая длина = 128)
   >
   > SECURE_PEPPER=... //Перец шифрования паролей
   > SECURE_ITERATIONS=... //Количество итераций шифрования паролей
   >
   > EMAIL_NAME=... //Отображаемое имя отправителя
   > EMAIL_EMAIL=... //Email почта (gmail.com)
   > EMAIL_PASSWORD=... //Пароль приложения для SMTP
   > EMAIL_CONFIRMMESSAGE_SUBJECT=... //Тема письма с подтверждением
   > EMAIL_CONFIRMMESSAGE_BODY=... //Наполнение пислма с подтверждением (форматирует {token} в токен подтверждения)
   >
   > API_URL=... //Адрес API
   > API_PORT_HTTP=... //Порт http API
   > API_PORT_HTTPS=... //Порт https API
   > WEB_PORT_HTTP=... //Порт http WEB приолжения
   > WEB_PORT_HTTPS=... //Порт https WEB приолжения
   > ```
* Запуск `docker-compose`
   ```sh
   docker-compose up
   ```

## Usage

После запуска `docker-compose` создается папка `./etc`.

В папке `./etc/db` находится файлы `MariaDB` базы данных в который происходит сохранение всех данных.