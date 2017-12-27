﻿namespace Bonsai.Code.DomainModel.Facts.Templates
{
    /// <summary>
    /// The template for specifying a relation between two entities.
    /// </summary>
    public class RelationFactTemplate: IFactTemplate
    {
        public string Name { get; set; }

        public string RelationTitle { get; set; }

        public string RelatedObjectKey { get; set; }
    }
}
