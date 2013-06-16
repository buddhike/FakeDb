﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FakeDb.Builtin.MaterializationHooks;

namespace FakeDb
{
    public class Db
    {
        readonly ICache _cache;
        readonly IIdPropertyFinder _idPropertyFinder;
        readonly ForeignKeyFinderFinder _foreignKeyFinder;
        readonly IList<IMaterializationHook> _materializationHooks;

        public Db(IIdGenerator idGenerator = null, IEnumerable<IMaterializationHook> materializationHooks = null)
        {
            _idPropertyFinder = new IdPropertyFinder();
            _foreignKeyFinder = new ForeignKeyFinderFinder();
            _materializationHooks = materializationHooks == null
                                        ? new List<IMaterializationHook>()
                                        : materializationHooks.ToList();

            _cache = new Cache(idGenerator ?? new IdGenerator(_idPropertyFinder), new ObjectGraph(), _materializationHooks);
        }

        public IInMemorySet Set<TEntity>() where TEntity : class
        {
            return _cache.For(typeof (TEntity));
        }

        public void RegisterForeignKeyInitializer()
        {
            _materializationHooks.Add(new ForeignKeyInitializer(_idPropertyFinder, _foreignKeyFinder));
        }

        public Db MapId<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> idPropExpr) where TEntity: class 
        {
            var idPropName = GetPropertyNameFromExpression(idPropExpr);
            
            _idPropertyFinder.RegisterIdName(typeof(TEntity), idPropName);

            return this;
        }

        public Db MapForeignKey<TEntity, TProperty, TForeignKey>(Expression<Func<TEntity, TForeignKey>> foeignKeyExpr, Expression<Func<TEntity, TProperty>> propExpr)
            where TEntity : class
            where TProperty : class
        {
            var propName = GetPropertyNameFromExpression(propExpr);
            var fkName = GetPropertyNameFromExpression(foeignKeyExpr);

            _foreignKeyFinder.RegisterForeignKey(typeof(TEntity), fkName, propName);

            return this;
        }

        static string GetPropertyNameFromExpression<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> expr)
        {
            try
            {
                return ((MemberExpression)expr.Body).Member.Name;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Invalid property expression. Please use the form of 'object.property'", ex);
            }
        }
    }
}