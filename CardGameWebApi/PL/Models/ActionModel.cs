﻿using CardGameWebApi.PL.Enums;

namespace CardGameWebApi.PL.Models {
    public class ActionModel
    {
        public int PlayerId { get; set; }

        public ActionTypes actionType { get; set; }

        public int CardId { get; set; }
    } } 