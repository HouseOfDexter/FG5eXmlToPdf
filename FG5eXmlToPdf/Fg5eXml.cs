﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;
using FG5eXmlToPdf;
using FG5eXmlToPdf.Models;
using FG5eXmlToPDF.Models;
// ReSharper disable All

namespace FG5eXmlToPDF
{
    public static class FG5eXml
    {

        public static Character5e LoadCharacter(string fileString)
        {
            var xml = XDocument.Load(fileString);
            _charElement = xml?.Root?.Element("character");


            var character = new Character5e();
            var props = character.Properities;
            foreach (var prop in props)
            {
                prop.Value = GetCharValue(prop.XmlPath);
            }
            //props.Add("AC", _charElement.XPathSelectElement("defenses/ac/total")?.Value ?? string.Empty);


            var abilityList = _charElement?.Element("abilities").Elements().ToList();
            foreach (var attrib in abilityList)
            {
                character.Abilities.Add(new Ability()
                {
                    Name = attrib.Name.ToString(),
                    Score = int.Parse(attrib.Element("score").Value),
                    Bonus = int.Parse(attrib.Element("bonus").Value),
                    Save = int.Parse(attrib.Element("save").Value),
                    Savemodifier = int.Parse(attrib.Element("savemodifier").Value),
                    Saveprof = Helper.StringIntToBool(attrib.Element("saveprof").Value)
                });
            }

            var skillList = _charElement?.Element("skilllist").Elements().ToList();
            foreach (var skill in skillList)
            {
                character.Skills.Add(new Skill()
                {
                    Name = skill.Element("name").Value,
                    Misc = int.Parse(skill.Element("misc").Value),
                    Prof = Helper.StringIntToBool(skill.Element("prof").Value),
                    Stat = skill.Element("stat").Value,
                    Total = int.Parse(skill.Element("total").Value)
                });
            }

            var classList = _charElement?.XPathSelectElement("classes").Elements().ToList();
            foreach (var charClass in classList)
            {
                character.Classes.Add(new Class()
                {
                    Name = charClass.Element("name").Value,
                    Level = charClass.Element("level").Value,
                });
            }

            var weaponList = _charElement?.XPathSelectElement("weaponlist").Elements().ToList();
            foreach (var weapon in weaponList)
            {
                var damageList = weapon.Element("damagelist").Elements().ToList();
                var damages = new List<Damage>();
                foreach (var danage in damageList)
                {
                    damages.Add(new Damage()
                    {
                        Type = danage.Element("type").Value,
                        Stat = danage.Element("stat").Value,
                        Dice = danage.Element("dice").Value,
                        Bonus = danage.Element("bonus").Value
                    });
                }
                character.Weapons.Add(new Weapon()
                {
                    Name = weapon.Element("name").Value,
                    AttackStat = weapon?.Element("attackstat")?.Value ?? String.Empty,
                    AttackBonus = weapon.Element("attackbonus").Value,
                    Type = weapon.Element("type").Value,
                    Damages = damages
                });
            }

            return character;
        }


        private static XElement _charElement;

        private static string GetCharValue(string name)
        {
            //everything is lowercase as far as I can tell in the xml
            return _charElement.XPathSelectElement(name.ToLower())?.Value.TrimStart('0') ?? string.Empty;
        }
    }
}
