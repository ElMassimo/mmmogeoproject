﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace USARoadTrip.Api.Infrastructure
{
    public interface IRepository<T> 
    {
        /// <summary>
        /// Checks wheter any existent entity matches the given condition.
        /// </summary>
        /// <param name="whereCondition">The condition to be tested</param>
        /// <returns>True if the condition is true for some entity</returns>
        bool Any(Expression<Func<T, bool>> whereCondition);

        /// <summary>
        /// Get a selected extiry by the object primary key ID
        /// </summary>
        /// <param name="id">Primary key ID</param>
        T GetSingle(Expression<Func<T, bool>> whereCondition);

        /// <summary> 
        /// Add entity to the repository 
        /// </summary> 
        /// <param name="entity">the entity to add</param> 
        /// <returns>The added entity</returns> 
        void Add(T entity);

        /// <summary> 
        /// Mark entity to be deleted within the repository 
        /// </summary> 
        /// <param name="entity">The entity to delete</param> 
        void Delete(T entity);

        /// <summary> 
        /// Updates entity within the the repository 
        /// </summary> 
        /// <param name="entity">the entity to update</param> 
        /// <returns>The updates entity</returns> 
        void Attach(T entity);

        /// <summary> 
        /// Load the entities using a linq expression filter
        /// </summary> 
        /// <typeparam name="E">the entity type to load</typeparam> 
        /// <param name="where">where condition</param> 
        /// <returns>the loaded entity</returns> 
        IList<T> GetAll(Expression<Func<T, bool>> whereCondition);

        /// <summary>
        /// Get all the element of this repository
        /// </summary>
        /// <returns></returns>
        IList<T> GetAll();

        /// <summary> 
        /// Query entities from the repository that match the linq expression selection criteria
        /// </summary> 
        /// <typeparam name="E">the entity type to load</typeparam> 
        /// <param name="where">where condition</param> 
        /// <returns>the loaded entity</returns> 
        IQueryable<T> GetQueryable();

        /// <summary>
        /// Count using a filer
        /// </summary>
        long Count(Expression<Func<T, bool>> whereCondition);

        /// <summary>
        /// All item count
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        long Count();

        void Save();
    }
}
