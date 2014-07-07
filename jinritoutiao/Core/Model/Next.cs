namespace jinritoutiao.Core.Model
{
    public class Next
    {
        private string _max_behot_time;
        private string _min_behot_time;


        public string MaxBehotTime
        {
            get { return _max_behot_time; }
            set { _max_behot_time = value; }
        }

        public string MinBehotTime
        {
            get { return _min_behot_time; }
            set { _min_behot_time = value; }
        }
    }
}
