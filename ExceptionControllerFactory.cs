using System.ComponentModel;
using System;

namespace exception
{
    public class ExceptionControllerFactory
    {
        private IListSource m_list;
        private IServerReporter m_server;

        public ExceptionControllerFactory() { }

        // инициализирует список критических исключений 
        public ExceptionControllerFactory WithList(IListSource src)
        {
            m_list = src;
            return this;
        }

        // инициализирует сервер
        public ExceptionControllerFactory WithServer(IServerReporter rep)
        {
            m_server = rep;
            return this;
        }

        // создает объект ExceptionController
        public ExceptionController Create()
        {
            return new ExceptionController(m_list, m_server);
        }
    }
}