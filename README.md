# blog API
### An API for blog apps like telegraph.io
- Used UnitOfWork and repository pattern for database related stuffs.
- Empowered to work with files such as images via POST and DELETE requests.
- Implemented the comments section.
- Each blog post holds its own images and comments privately. When one of the post is deleted, all its data is also deleted from database tables. There used one-to-many relationship.
![Photo](/api/BLOGAPI.png)
