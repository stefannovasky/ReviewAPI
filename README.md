# ReviewApi 
:star: REST API where users can analyze, evaluate and give their opinion about anything.

[author-link]:https://img.shields.io/badge/Author-Stefan%20Novasky-brightgreen
[license-link]:https://img.shields.io/badge/License-MIT-green
[star-link]:https://img.shields.io/github/stars/stefannovasky/ReviewApi?color=green

[![author][author-link]](https://github.com/stefannovasky)
[![license][license-link]](https://github.com/stefannovasky/ReviewAPI/blob/master/LICENSE)
[![license][star-link]](https://github.com/stefannovasky/ReviewAPI)


# :computer: Technologies 
- ASP .NET Core 3.1
- SQL Server
- Entity Framework Core
- Redis

# :running: Running
1. Clone this repository:
    `git clone https://github.com/stefannovasky/ReviewAPI.git`
2. Configure:
    Configure SQL Server, Redis and Smtp Server connection on src/ReviewApi/ReviewApi/appsettings.json 
3. Start Redis Server on the terminal:
    `redis-server`
4. Start the application on the terminal
    `dotnet run --project ./src/ReviewApi/ReviewApi/ReviewApi.csproj`

# :rocket: Features
- register user
- authenticate the user
- the user can recover his password
- the authenticated user can delete his account
- the authenticated user can update his name, password and profile picture
- the authenticated user can see my profile 
- the authenticated user can see the profile of other users
- authenticated user can see other users' favorite opinions
- the user can list opinions
- authenticated user can see their opinions
- authenticated user can see their favorite opinions
- authenticated user can create a revision
- the authenticated user can update his revision
- the authenticated user can delete his revision
- authenticated user can favorite a review 
- authenticated user can remove favorite from a review
- authenticated user can comment on a revision
- the authenticated user can update his/her comment 
- the authenticated user can delete his/her comment 

# :memo: Documentation
Run the application and access the route: */swagger/index.html*

# :tada: Contributing
If you want to contribute please fork the repository and get your hands dirty, and make the changes as you'd like and submit the Pull Request.

# :closed_book: License 
This project is under [MIT license](https://github.com/stefannovasky/ReviewAPI/blob/master/LICENSE).