using System;
using System.Collections.Generic;
using ORMapper_Framework;
using ORMapper_Framework.Enums;
using ORMapper_Framework.Queries;
using ORMapperDemo.Library;

namespace ORMapperDemo
{
    public static class Showcase
    {
        /// <summary>
        /// Creates tables in the database
        /// </summary>
        public static void CreateTables()
        {
            Console.WriteLine("(1) Creating tables");
            Console.WriteLine("-----------------");

            Console.WriteLine("-> Register SpecialCustomer and other entities recursively");
            // Register entities recursively
            OrMapper.RegisterNewEntity<SpecialCustomer>();

            try
            {
                Console.WriteLine("-> Drop tables in transaction");
                // Drop tables in a transaction
                OrMapper.StartTransaction();
                OrMapper.EnsureDeleted();
                OrMapper.CommitTransaction();
                Console.WriteLine("-> Transaction committed\n");
            }
            catch (OrMapperDatabaseException e)
            {
                // Roll back if ORMapper is in transaction and database exception occurs
                if (OrMapper.InTransaction)
                {
                    Console.WriteLine("-> Rolling back transaction");
                    OrMapper.RollbackTransaction();
                }
                throw;
            }

            try
            {
                Console.WriteLine("-> Create tables in transaction");
                // Create tables in a transaction
                OrMapper.StartTransaction();
                OrMapper.EnsureCreated();
                OrMapper.CommitTransaction();
                Console.WriteLine("-> Transaction committed\n");
            }
            catch (OrMapperDatabaseException e)
            {
                // Roll back if ORMapper is in transaction and database exception occurs
                if (OrMapper.InTransaction)
                {
                    Console.WriteLine("-> Rolling back transaction");
                    OrMapper.RollbackTransaction();
                }
                throw;
            }
            
        }
        /// <summary>
        /// Insert data showcase
        /// </summary>
        public static void InsertData()
        {
            Console.WriteLine("(2) Inserting data");
            Console.WriteLine("-----------------");
            try
            {
                Console.WriteLine("-> Create objects in transaction");
                // Insert data 
                OrMapper.StartTransaction();
                
                // Genres
                Genre horror = new Genre() {Id = 1, GenreName = "Horror"};
                Genre fantasy = new Genre() {Id = 2, GenreName = "Fantasy"};
                Genre adventure = new Genre() {Id = 3, GenreName = "Adventure"};

                // Books
                Book horrorBook1 = new Book()
                {
                    BookID = "h1",
                    Genre = horror,
                    Title = "Frankenstein"
                };
                Book horrorBook2 = new Book()
                {
                    BookID = "h2",
                    Genre = horror,
                    Title = "Dracula"
                };
                Book fantasyBook = new Book()
                {
                    BookID = "f1",
                    Genre = fantasy,
                    Title = "Der Herr der Ringe"
                };
                Book adventureBook = new Book()
                {
                    BookID = "ad1",
                    Genre = adventure,
                    Title = "Die Tribute von Panem"
                };
                // Authors
                Author author1 = new Author()
                {
                    AbbreviatedName = "J. R. R. Tolkien",
                    BirthDate = new DateTime(1892, 1, 3),
                    Gender = EGender.Male,
                    Books = new List<Book>() {fantasyBook},
                    ID = 1,
                    Name = "John Ronald Reuel",
                    SurName = "Tolkien"
                };
                Author author2 = new Author()
                {
                    AbbreviatedName = "S. Collins",
                    BirthDate = new DateTime(1962, 8, 10),
                    Gender = EGender.Female,
                    Books = new List<Book>() { adventureBook },
                    ID = 2,
                    Name = "Suzanne",
                    SurName = "Collins"
                };
                Author author3 = new Author()
                {
                    AbbreviatedName = "B. Stoker",
                    BirthDate = new DateTime(1847, 11, 8),
                    Gender = EGender.Male,
                    Books = new List<Book>() { horrorBook2 },
                    ID = 3,
                    Name = "Bram",
                    SurName = "Stoker"
                };
                Author author4 = new Author()
                {
                    AbbreviatedName = "M. Shelley",
                    BirthDate = new DateTime(1797, 8, 30),
                    Gender = EGender.Female,
                    Books = new List<Book>() { horrorBook1 },
                    ID = 4,
                    Name = "Mary",
                    SurName = "Shelley"
                };
                // Special customers
                SpecialCustomer specialCustomer1 = new SpecialCustomer()
                {
                    BirthDate = new DateTime(1998, 9, 30),
                    Books = new List<Book>() {horrorBook1, horrorBook2},
                    Gender = EGender.Male,
                    ID = 5,
                    Name = "Daniel",
                    SurName = "Huber",
                    Password = "Secret123",
                    RegisteredSince = new DateTime(2015, 11, 24)
                };
                SpecialCustomer specialCustomer2 = new SpecialCustomer()
                {
                    BirthDate = new DateTime(1964, 12, 23),
                    Books = new List<Book>() { adventureBook, fantasyBook },
                    Gender = EGender.Female,
                    ID = 6,
                    Name = "Fritz",
                    SurName = "Franz",
                    Password = "FranzFritz",
                    RegisteredSince = new DateTime(2010, 9, 22)
                };
                // Save genres
                Console.WriteLine("-> Save genres");
                OrMapper.Save(horror);
                OrMapper.Save(fantasy);
                OrMapper.Save(adventure);
                // Save books
                Console.WriteLine("-> Save books");
                OrMapper.Save(horrorBook1);
                OrMapper.Save(horrorBook2);
                OrMapper.Save(fantasyBook);
                OrMapper.Save(adventureBook);
                // Save authors
                Console.WriteLine("-> Save authors");
                OrMapper.Save(author1);
                OrMapper.Save(author2);
                OrMapper.Save(author3);
                OrMapper.Save(author4);
                // Save special customers
                Console.WriteLine("-> Save special customers");
                OrMapper.Save(specialCustomer1);
                OrMapper.Save(specialCustomer2);

                // Save method automatically inserts data for inherited fields in the parent tables

                OrMapper.CommitTransaction();
                Console.WriteLine("-> Committed transaction\n");
                // Clear cache afterwards (-> otherwise genre.Books for example would be empty for the following showcases | after the cache is cleared all relations of a read object will be filled correctly / completely)
            }
            catch (OrMapperDatabaseException e)
            {
                // Roll back if ORMapper is in transaction and database exception occurs
                if (OrMapper.InTransaction)
                {
                    Console.WriteLine("-> Rolling back transaction");
                    OrMapper.RollbackTransaction();
                }
                throw;
            }
        }
        /// <summary>
        /// Rollback showcase
        /// </summary>
        public static void RollbackExample()
        {
            Console.WriteLine("(3) Rollback demonstration");
            Console.WriteLine("-----------------");

            try
            {
                Console.WriteLine("-> Insert data that violates UNIQUE constraint of genre table");
                // Insert data 
                OrMapper.StartTransaction();
                // New genre that violates the UNIQUE Constraint on genre name
                Genre horror = new Genre() { Id = 4, GenreName = "Horror" };
                OrMapper.Save(horror);
                OrMapper.CommitTransaction();
            }
            catch (OrMapperDatabaseException e)
            {
                // Roll back because UNIQUE constraint got violated
                if (OrMapper.InTransaction)
                {
                    Console.WriteLine("-> Roll back transaction after database exception was thrown");
                    OrMapper.RollbackTransaction();
                }
                    
            }

            Console.WriteLine("-> Now insert data that doesn't violate constraints etc.");

            try
            {
                // Insert data 
                OrMapper.StartTransaction();
                // New genre that does not violate the UNIQUE Constraint on genre name
                Genre scifi = new Genre() { Id = 4, GenreName = "Sci-Fi" };
                OrMapper.Save(scifi);
                Console.WriteLine("-> Roll it back manually\n");
                OrMapper.RollbackTransaction();
            }
            catch (OrMapperDatabaseException e)
            {
                // Roll back because UNIQUE constraint got violated
                if (OrMapper.InTransaction)
                {
                    Console.WriteLine("-> Roll back transaction");
                    OrMapper.RollbackTransaction();
                }

                throw;
            }
        }

        /// <summary>
        /// Update data demonstration (with locking)
        /// </summary>
        public static void UpdateData()
        {
            Console.WriteLine("(4) Update data");
            Console.WriteLine("-----------------");

            try
            {
                Console.WriteLine("-> Load a book");
                // Create tables in a transaction
                OrMapper.StartTransaction();
                Book book = OrMapper.Read<Book>("h2");
                Console.WriteLine("-> Lock the book to prevent influences from other transactions (FOR UPDATE)");
                OrMapper.LockObject(book, LockType.ForUpdate);
                // Load all authors
                Console.WriteLine("-> Load all authors and lock them (FOR SHARE)");
                List<Author> authors = new Query<Author>().Execute();
                authors.ForEach(a=>OrMapper.LockObject(a, LockType.ForShare));
                Console.WriteLine("-> Load other genre for book and lock it (FOR KEY SHARE)");
                Genre genre = OrMapper.Read<Genre>(3);
                OrMapper.LockObject(genre,LockType.ForKeyShare);
                Console.WriteLine("-> Set new authors and genre for book and save changes to database");
                book.Authors = authors;
                book.Genre = genre;
                OrMapper.Save(book);
                Console.WriteLine("-> Commit transaction and release all locks");
                OrMapper.CommitTransaction();
                Console.WriteLine("-> Transaction committed\n");
            }
            catch (OrMapperDatabaseException e)
            {
                // Roll back if ORMapper is in transaction and database exception occurs
                if (OrMapper.InTransaction)
                {
                    Console.WriteLine("-> Rolling back transaction");
                    OrMapper.RollbackTransaction();
                }
                throw;
            }

        }
        /// <summary>
        /// Delete showcase
        /// </summary>
        public static void DeleteData()
        {
            Console.WriteLine("(5) Delete data");
            Console.WriteLine("-----------------");

            try
            {
                Console.WriteLine("-> Insert new genre");
                OrMapper.StartTransaction();
                Genre romance = new Genre() { Id = 4, GenreName = "Romance" };
                Console.WriteLine("-> Save new genre");
                OrMapper.Save(romance);
                Console.WriteLine("-> Load new genre");
                romance = OrMapper.Read<Genre>(4);
                Console.WriteLine("-> Delete the new genre\n");
                OrMapper.Delete(romance);
                OrMapper.CommitTransaction();
            }
            catch (OrMapperDatabaseException e)
            {
                Console.WriteLine("-> Delete showcase could not be conducted");
                throw;
            }
        }
        /// <summary>
        /// One to many relationship showcase
        /// </summary>
        public static void OneToMany()
        {
            Console.WriteLine("(6) One to many showcase");
            Console.WriteLine("-----------------");
            try
            {
                Console.WriteLine("-> Retrieve a genre and a book");
                OrMapper.StartTransaction();
                Genre adventure = OrMapper.Read<Genre>(3);
                Book horror1 = OrMapper.Read<Book>("h1");
                Console.WriteLine($"-> Books related to genre {adventure.GenreName}");
                adventure.Books.ForEach(b=>Console.WriteLine($"--> {b.BookID}: {b.Title}"));
                Console.WriteLine("-> Add book to genre");
                adventure.Books.Add(horror1);
                Console.WriteLine("-> Save changes");
                OrMapper.Save(adventure);
                Console.WriteLine("-> Reload genre (clear cache first to prevent loading a cached object)");
                OrMapper.ClearCache();
                adventure = OrMapper.Read<Genre>(3);
                Console.WriteLine($"-> Books related to genre {adventure.GenreName}");
                adventure.Books.ForEach(b => Console.WriteLine($"--> {b.BookID}: {b.Title}"));
                Console.WriteLine("-> Finish 1:n showcase\n");
                OrMapper.CommitTransaction();
            }
            catch (OrMapperDatabaseException e)
            {
                if (OrMapper.InTransaction)
                {
                    Console.WriteLine("-> Rolling back transaction");
                    OrMapper.RollbackTransaction();
                }
                throw;
            }
        }

        /// <summary>
        /// Many to many showcase
        /// </summary>
        public static void ManyToMany()
        {
            Console.WriteLine("(7) Many to many showcase");
            Console.WriteLine("-----------------");

            try
            {
                Console.WriteLine("-> Retrieve a book");
                OrMapper.StartTransaction();
                Book horror2 = OrMapper.Read<Book>("h2");
                Console.WriteLine($"-> Authors of book {horror2.Title}");
                horror2.Authors.ForEach(a=>Console.WriteLine($"--> {a.AbbreviatedName}"));
                Console.WriteLine("-> Remove all authors and save it");
                horror2.Authors.Clear();
                OrMapper.Save(horror2);
                Console.WriteLine("-> Add correct author and save changes to database");
                Author author = OrMapper.Read<Author>(3);
                horror2.Authors.Add(author);
                OrMapper.Save(horror2);
                Console.WriteLine("-> Clear cache and load m:n relation from author table");
                OrMapper.ClearCache();
                Author bram = OrMapper.Read<Author>(3);
                Console.WriteLine($"-> Author {bram.AbbreviatedName} wrote:");
                bram.Books.ForEach(b=>Console.WriteLine($"--> {b.BookID}: {b.Title}"));
                Book dracula = OrMapper.Read<Book>("h2");
                Console.WriteLine($"-> Authors of book {dracula.Title}");
                dracula.Authors.ForEach(a => Console.WriteLine($"--> {a.AbbreviatedName}"));
                Console.WriteLine("-> Finish m:n showcase\n");
                OrMapper.CommitTransaction();
            }
            catch (OrMapperDatabaseException e)
            {
                if (OrMapper.InTransaction)
                {
                    Console.WriteLine("-> Rolling back transaction");
                    OrMapper.RollbackTransaction();
                }
                throw;
            }
        }
        /// <summary>
        /// Queries showcase
        /// </summary>
        public static void Queries()
        {
            Console.WriteLine("(8) Queries showcase");
            Console.WriteLine("-----------------");

            Console.WriteLine("-> All Authors (base query):");
            var allAuthors = new Query<Author>().Execute();
            allAuthors.ForEach(a=>Console.WriteLine($"--> {a.ID} {a.Name} {a.SurName} ({a.AbbreviatedName})"));
            Console.WriteLine("");
            Console.WriteLine("-> All Books (base query):");
            var allBooks = new Query<Book>().Execute();
            allBooks.ForEach(b =>
            {
                Console.WriteLine($"--> {b.BookID} - {b.Title} ({b.Genre.GenreName})");
                Console.WriteLine("---> Written by:");
                b.Authors.ForEach(a=>Console.WriteLine($"----> {a.AbbreviatedName}"));
                
            });
            Console.WriteLine("");
            Console.WriteLine("-> Only adventure books (WHERE + EQUALS):");
            var adventureBooks = new Query<Book>().Where().Equals("GID", OrMapper.Read<Genre>(3)).Execute();
            adventureBooks.ForEach(b =>
            {
                Console.WriteLine($"--> {b.BookID} - {b.Title} ({b.Genre.GenreName})");

            });

            Console.WriteLine("");
            Console.WriteLine("-> Not adventure books (WHERE + NOT EQUALS):");
            var notadventureBooks = new Query<Book>().Where().Equals("GID", OrMapper.Read<Genre>(3), true).Execute();
            notadventureBooks.ForEach(b =>
            {
                Console.WriteLine($"--> {b.BookID} - {b.Title} ({b.Genre.GenreName})");

            });

            Console.WriteLine("");
            Console.WriteLine("-> Authors with ID greater than 2 (WHERE + GREATER THAN):");
            var gtAuthors = new Query<Author>().Where().GreaterThan("ID", 2).Execute();
            gtAuthors.ForEach(a =>
            {
                Console.WriteLine($"--> {a.ID} - {a.AbbreviatedName}");
            });

            Console.WriteLine("");
            Console.WriteLine("-> Authors with ID less than 2 or id in list(WHERE + LESS THAN + OR + IN):");
            var ltAuthors = new Query<Author>().Where().LessThan("ID", 2).Or().In("ID", new List<object>(){3,4}).Execute();
            ltAuthors.ForEach(a =>
            {
                Console.WriteLine($"--> {a.ID} - {a.AbbreviatedName}");
            });

            Console.WriteLine("");
            Console.WriteLine("-> Special customers with ID less than 6 and password / name not null(WHERE + LESS THAN + AND + NOT + BEGINSET, ENDSET + IS NULL):");
            var specialCustomers = new Query<SpecialCustomer>().Where().LessThan("ID", 6).And().Not().BeginSet().IsNull("NAME").Or().IsNull("PASSWORD").EndSet().Execute();
            specialCustomers.ForEach(s =>
            {
                Console.WriteLine($"--> {s.ID} - {s.Name} + {s.Password}");
            });

            Console.WriteLine("");
            Console.WriteLine("-> People with ID between 3 and 5 (BETWEEN):");
            var people = new Query<Person>().Where().Between("ID", 3, 5).Execute();
            people.ForEach(p =>
            {
                Console.WriteLine($"--> {p.ID} - {p.Name}");
            });

            Console.WriteLine("");
            Console.WriteLine("-> Books that match the pattern %cul_ (LIKE):");
            var culBooks = new Query<Book>().Where().Like("TITLE", "%cul_").Execute();
            culBooks.ForEach(b =>
            {
                Console.WriteLine($"--> {b.BookID} - {b.Title}");
            });
        }

    }
}