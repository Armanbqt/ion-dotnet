﻿/*
 * Copyright 2020 Amazon.com, Inc. or its affiliates. All Rights Reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License").
 * You may not use this file except in compliance with the License.
 * A copy of the License is located at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * or in the "license" file accompanying this file. This file is distributed
 * on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either
 * express or implied. See the License for the specific language governing
 * permissions and limitations under the License.
 */

namespace Amazon.IonDotnet
{
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// A symbol table maps symbols between their textual form and an integer ID used in the binary encoding.
    /// </summary>
    public interface ISymbolTable
    {
        /// <summary>
        /// Gets the unique name of this symbol table.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the version of this symbol table.
        /// </summary>
        int Version { get; }

        /// <summary>
        /// Gets a value indicating whether if this table is local,
        /// and therefore unversioned and unnamed.
        /// </summary>
        /// <remarks>
        /// If this is true, then IsShared() and IsSystem() will return false.
        /// </remarks>
        bool IsLocal { get; }

        /// <summary>
        /// Gets a value indicating whether if this table is shared,
        /// and therefore named, versioned, and read-only.
        /// </summary>
        bool IsShared { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is substituting for an imported shared table for which no exact match was found in the catalog.
        /// Such tables are not authoritative and may not even have any symbol text at all (for ex when no version of an imported table is found).
        /// </summary>
        /// <remarks> Substitute tables are always shared, non-system tables.</remarks>
        bool IsSubstitute { get; }

        /// <summary>
        /// Gets a value indicating whether if this table is a system table, and therefore shared, named, versioned, and read-only.
        /// </summary>
        bool IsSystem { get; }

        /// <summary>
        /// Gets a value indicating whether if this table can have symbols added to it.
        /// </summary>
        /// <remarks> Shared symtabs are always read-only.</remarks>
        bool IsReadOnly { get; }

        /// <summary>
        /// Gets the identifier for the Ion version (and thus the system symbol table) used by this table.
        /// </summary>
        /// <remarks>The version identifier is a string of the form "ion_X_Y".</remarks>
        string IonVersionId { get; }

        /// <summary>
        /// Gets the highest symbol id reserved by this table.
        /// </summary>
        /// <remarks>
        /// Return the largest integer such that findKnownSymbol(int) could return a non-null result.
        /// Note that there is no promise that it will return a name, only that any larger id will not have a name defined.
        /// </remarks>
        int MaxId { get; }

        /// <summary>
        /// Prevents this table from accepting any more new symbols.
        /// </summary>
        /// <remarks>
        /// Making a local table read-only enables some optimizations when writing data, but will cause failures if new symbols are encountered.
        /// Shared symtabs are always read-only.
        /// </remarks>
        void MakeReadOnly();

        /// <summary>
        /// Gets the system symbol table being used by this local table.
        /// </summary>
        /// <returns>Null for non-system shared table, otherwise the system table.</returns>
        ISymbolTable GetSystemTable();

        IReadOnlyList<ISymbolTable> GetImportedTables();

        /// <summary>
        /// Gets the highest symbol id reserved by this table's imports (including system symbols).
        /// </summary>
        /// <returns>Max imported id.</returns>
        /// <remarks>
        /// Any id higher than this value is a local symbol declared by this table. This value is zero for shared symbol tables,
        /// since they do not utilize imports.
        /// </remarks>
        int GetImportedMaxId();

        /// <summary>
        /// Adds a new symbol to this table, or finds an existing definition of it.
        /// </summary>
        /// <param name="text">text the symbol text to intern.</param>
        /// <returns>the interned symbol, with both text and SID defined; not null.</returns>
        SymbolToken Intern(string text);

        /// <summary>
        /// Finds a symbol already interned by this table.
        /// </summary>
        /// <param name="text">The symbol text to find.</param>
        /// <returns>The interned symbol, with both text and SID defined; or the (null,-1) token if not interned.</returns>
        SymbolToken Find(string text);

        /// <summary>
        /// Gets the symbol ID associated with a given symbol name.
        /// </summary>
        /// <param name="text">Symbol name.</param>
        /// <returns>The id of the requested symbol or <see cref="SymbolToken.UnknownSid"/> if not defined.</returns>
        int FindSymbolId(string text);

        /// <summary>
        /// Gets the interned text for a symbol ID.
        /// </summary>
        /// <param name="sid">The requested symbol ID.</param>
        /// <returns>The interned text associated with the symbol ID, or Null if undefined.</returns>
        string FindKnownSymbol(int sid);

        /// <summary>
        /// Writes an Ion representation of this symbol table.
        /// </summary>
        /// <param name="writer">The IIonWriter to write to.</param>
        void WriteTo(IIonWriter writer);

        /// <summary>
        /// Creates an <see cref="IEnumerable"/> that will return all non-imported symbol names, in order of their symbol IDs.
        /// The iterator will return Null where there is an undefined sid.
        /// </summary>
        /// <returns>Own symbol names of this table.</returns>
        IEnumerable<string> GetDeclaredSymbolNames();
    }
}
