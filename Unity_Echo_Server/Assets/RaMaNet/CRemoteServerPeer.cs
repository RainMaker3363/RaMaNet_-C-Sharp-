using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameServer_Module;
using RaMaNetUnity;

namespace RaMaNetUnity
{
    // 클라이언트에서 통신을 수행할 대상이 되는 서버 객체
    public class CRemoteServerPeer : IPeer
    {
        public CUserToken token { get; private set; }
        WeakReference ramanet_manager;

        public CRemoteServerPeer(CUserToken token)
        {
            this.token = token;
            this.token.set_peer(this);
        }

        public void set_eventmanager(CRaMaNetEventManager event_manager)
        {
            this.ramanet_manager = new WeakReference(event_manager);
        }

        // 메시지를 수신했을 때 호출된다.
        // 파라미터로 넘오온 버퍼는 워커 스레드에서 재사용 되므로 복사한 뒤 어플리케이션으로 넘겨준다
        void IPeer.on_message(Const<byte[]> buffer)
        {
            // 버퍼를 복사한 뒤 CPacket 클래스로 감싼 뒤 넘겨준다.
            // CPacket 클래스 내부에서는 참조로만 들고 있는다.
            byte[] app_buffer = new byte[buffer.Value.Length];
            Array.Copy(buffer.Value, app_buffer, buffer.Value.Length);

            CPacket msg = new CPacket(app_buffer, this);

            (this.ramanet_manager.Target as CRaMaNetEventManager).enqueue_network_message(msg);
        }

        void IPeer.on_removed()
        {
            (this.ramanet_manager.Target as CRaMaNetEventManager).enqueue_network_event(NETWORK_EVENT.disconnected);
        }

        void IPeer.send(CPacket msg)
        {
            this.token.send(msg);
        }

        void IPeer.disconnect()
        {
        }

        void IPeer.process_user_operation(CPacket msg)
        {
        }
    }
}

