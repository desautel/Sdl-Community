﻿using System.Collections.Generic;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.ApplyTMTemplate.Models
{
	public class Template
	{
		public Template(List<LanguageResourceBundle> resourceBundle)
		{
			ResourceBundles = resourceBundle;
		}

		public List<LanguageResourceBundle> ResourceBundles { get; set; }

		public void ApplyTmTemplate(List<TranslationMemory> translationMemories)
		{
			foreach (var languageResourceBundle in ResourceBundles)
			{
				foreach (var translationMemory in translationMemories)
				{
					translationMemory.ApplyTemplate(languageResourceBundle);
				}
			}
		}
	}
}