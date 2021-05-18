using System.ComponentModel;
using System;

namespace exception
{
    public class ExceptionControllerFactory
    {
        private IListSource m_list;
        private IServerReporter m_server;

        public ExceptionControllerFactory() { }

        public ExceptionControllerFactory WithList(IListSource src)
        {
            m_list = src;
            return this;
        }

        public ExceptionControllerFactory WithServer(IServerReporter rep)
        {
            m_server = rep;
            return this;
        }

        // ExceptionController object creation
        public ExceptionController Create()
        {
            return new ExceptionController(m_list, m_server);
        }
    }
}