﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Assets.ThirdParty.Spriter2Unity.Editor.Spriter
{
    public class Entity : KeyElem
    {
        public const string XmlKey = "entity";

		public ScmlObject Scml { get; private set;}
        public string Name { get; private set; }
        public IEnumerable<Animation> Animations { get { return animations; } }

        public Entity(XmlElement element, ScmlObject scml)
            : base(element)
        { 
			Parse (element, scml);
		}

        protected virtual void Parse(XmlElement element, ScmlObject scml)
		{
			Scml = scml;

            Name = element.GetString("name", "");

            LoadAnimations(element);
        }

        private void LoadAnimations(XmlElement element)
        {
            var animElements = element.GetElementsByTagName(Animation.XmlKey);
            foreach (XmlElement animElement in animElements)
            {
                animations.Add(new Animation(animElement, this));
            }
        }

        private List<Animation> animations = new List<Animation>();
    }
}
