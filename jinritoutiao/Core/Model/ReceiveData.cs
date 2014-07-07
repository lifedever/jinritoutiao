using System;

namespace jinritoutiao.Core.Model
{
    public class ReceiveData
    {
        private string _abstract;
        private string _middleMode;
        private string _datetime;
        private string _moreMode;
        private string _createTime;
        private string _isFavorite;
        private long _id;
        private string _favoriteCount;
        private int _buryCount;
        private string _title;
        private string _source;
        private int _commentCount;
        private int _commentsCount;
        private int _isDigg;
        private string _displayTime;
        private string _publishTime;
        private int _goDetailCount;
        private string _sourceUrl;
        private Boolean _largeMode;
        private int _repinCount;
        private string _displayUrl;
        private int _diggCount;
        private string _behotTime;
        private string _imageUrl;
        private int _isBury;
        private long _groupId;
        private int _imageCount = 0;
        private MiddleImage _middleImage;

        public string Abstract
        {
            get { return _abstract; }
            set { _abstract = value; }
        }

        public string MiddleMode
        {
            get { return _middleMode; }
            set { _middleMode = value; }
        }

        public string Datetime
        {
            get { return _datetime; }
            set { _datetime = value; }
        }

        public string MoreMode
        {
            get { return _moreMode; }
            set { _moreMode = value; }
        }


        public string IsFavorite
        {
            get { return _isFavorite; }
            set { _isFavorite = value; }
        }

        public long Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string FavoriteCount
        {
            get { return _favoriteCount; }
            set { _favoriteCount = value; }
        }

        public int BuryCount
        {
            get { return _buryCount; }
            set { _buryCount = value; }
        }

        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        public string Source
        {
            get { return _source; }
            set { _source = value; }
        }

        public int CommentCount
        {
            get { return _commentCount; }
            set { _commentCount = value; }
        }

        public int CommentsCount
        {
            get { return _commentsCount; }
            set { _commentsCount = value; }
        }

        public int IsDigg
        {
            get { return _isDigg; }
            set { _isDigg = value; }
        }

        public string DisplayTime
        {
            get { return _displayTime; }
            set { _displayTime = value; }
        }

        public string PublishTime
        {
            get { return _publishTime; }
            set { _publishTime = value; }
        }

        public int GoDetailCount
        {
            get { return _goDetailCount; }
            set { _goDetailCount = value; }
        }

        public string SourceUrl
        {
            get { return _sourceUrl; }
            set { _sourceUrl = value; }
        }

        public bool LargeMode
        {
            get { return _largeMode; }
            set { _largeMode = value; }
        }

        public int RepinCount
        {
            get { return _repinCount; }
            set { _repinCount = value; }
        }

        public string DisplayUrl
        {
            get { return _displayUrl; }
            set { _displayUrl = value; }
        }

        public int DiggCount
        {
            get { return _diggCount; }
            set { _diggCount = value; }
        }

        public string BehotTime
        {
            get { return _behotTime; }
            set { _behotTime = value; }
        }

        public string ImageUrl
        {
            get { return _imageUrl; }
            set { _imageUrl = value; }
        }

        public int IsBury
        {
            get { return _isBury; }
            set { _isBury = value; }
        }

        public long GroupId
        {
            get { return _groupId; }
            set { _groupId = value; }
        }

        public MiddleImage MiddleImage
        {
            get { return _middleImage; }
            set { _middleImage = value; }
        }

        public int ImageCount
        {
            get { return _imageCount; }
            set { _imageCount = value; }
        }

        public string CreateTime
        {
            get { return _createTime; }
            set { _createTime = value; }
        }
    }
}
