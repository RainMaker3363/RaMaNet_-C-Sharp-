using System;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameServer_Module
{
    class SocketAsyncEventArgsPool
    {
        Stack<SocketAsyncEventArgs> m_pool;

        // 오브젝트 풀을 적정 사이즈에 맞게 초기화 합니다.
        public SocketAsyncEventArgsPool(int capacity)
        {
            m_pool = new Stack<SocketAsyncEventArgs>(capacity);
        }

        // SocketAsyncEventArg 인스턴스 풀에 추가한다.
        public void Push(SocketAsyncEventArgs item)
        {
            if(item == null)
            {
                throw new ArgumentNullException("Items Added to a SocketAsyncEventArgsPool cannot be Null");
            }

            lock(m_pool)
            {
                m_pool.Push(item);
            }
        }

        // SocketAsyncEventArg 인스턴스 풀에 요소를 꺼낸다.
        public SocketAsyncEventArgs Pop()
        {
            lock(m_pool)
            {
                return m_pool.Pop();
            }
        }

        // SocketAsyncEventArg 인스턴스 풀에 요소의 수
        public int count
        {
            get
            {
                return m_pool.Count;
            }
        }
    }
}
