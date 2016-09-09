using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using GameServer_Module;

public enum PROTOCOL : short
{
    BEGIN = 0,

    CHAT_MSG_REQ = 1,
    CHAT_MSG_ACK = 2,

    END
}

namespace RaMaNetUnity
{


    // 네트워크 라이브러리와 유니티 프로젝트를 연결해주는 기능
    // 기존 서버 모듈은 송, 수신 스레드가 별개로 나뉘어 작업을 하지만 유니티는 싱글 스레드 기반으로 되어있기에
    // 이것을 조정하고 호환되게 만들어주는 역할이다.
    public class CRaMaNetUnityService : MonoBehaviour
    {

        CRaMaNetEventManager event_manager;

        // 연결된 게임 서버 객체
        IPeer gameserver;

        // TCP 통신을 위한 서비스 객체
        CNetworkService service;

        // 네트워크 상태 변경 시 호출되는 델리게이트, 어플레키에션에서 콜백 메소드를 설정하여 사용한다.
        public delegate void StatusChangedHandler(NETWORK_EVENT status);
        public StatusChangedHandler appcallbackon_status_changed;

        // 네트워크 메시지 수신 시 호출되는 델리게이트. 어플리케이션에서 콜백 메소드를
        // 설정하여 사용한다.
        public delegate void MessageHandler(CPacket msg);
        public MessageHandler appcallback_on_message;

        void Awake()
        {
            CPacketBufferManager.initialize(10);

            // CRaMaNetEventManager 객체는 MonoBehaviour를 상속받은 객체가 아니므로 new를 선언하여 만들어준다.
            this.event_manager = new CRaMaNetEventManager();
        }

        // Use this for initialization
        //void Start()
        //{

        //}

        // Update is called once per frame
        // 네트워크에서 발생하는 모든 이벤트를 클라이언트에 알려주는 역할을 Update에서 진행한다.
        // RaMaNet 엔진의 메시지 송, 수신 처리는 워커 스레드에서 수행되지만
        // 유니티의 로직 처리는 메인 스레드에서 수행되므로
        // 큐잉 처리를 통하여 메인 스레드에서 모든 로직 처리가 이루어 지도록 구성하였다.

        void Update()
    {
        // 수신된 메시지에 대한 콜백.
        if(this.event_manager.has_message())
        {
            CPacket msg = this.event_manager.dequeue_network_meesage();

            if(this.appcallback_on_message != null)
            {
                this.appcallback_on_message(msg);
            }
        }
	
        // 네트워크 발생 이벤트에 대한 콜백
        if(this.event_manager.has_event())
        {
            NETWORK_EVENT status = this.event_manager.dequeue_network_event();

            if(this.appcallbackon_status_changed != null)
            {
                this.appcallbackon_status_changed(status);
            }
        }
	}

        public void connect(string host, int port)
        {
            // CNetworkService 객체는 메시지의 비동기 송, 수신 처리를 수행한다.
            this.service = new CNetworkService();

            // endpoint 정보를 갖고있는 Connector 생성. 만들어둔 NetWorkService 객체를 넣어준다.
            CConnector connector = new CConnector(service);

            // 접속 성공 시 호출될 콜백 메소드 지정.
            connector.connected_callback += on_connected_gameserver;
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse(host), port);
            connector.connect(endpoint);
        }

        public void send(CPacket msg)
        {
            try
            {
                this.gameserver.send(msg);
                CPacket.destroy(msg);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        // 접속 성공 시 호출될 콜백 메소드
        void on_connected_gameserver(CUserToken server_token)
        {
            this.gameserver = new CRemoteServerPeer(server_token);
            ((CRemoteServerPeer)this.gameserver).set_eventmanager(this.event_manager);

            // 유니티 어플리케이션으로 이벤트를 넘겨주기 위해서 매니저에 큐잉시켜 준다.
            this.event_manager.enqueue_network_event(NETWORK_EVENT.connected);
        }
    }
}

