export default function getStyle(data) {
    let ratio = 10;
    return {
        container: {
            width: data.width,
            height: data.height,
            borderRadius: 12,
            paddingLeft: data.width * 0.03,
            paddingRight: data.width * 0.03,
        },
        rankList: {
            width: Math.ceil(data.width * 0.94),
            height: data.height,
        },
        list: {
            width: Math.ceil(data.width * 0.94),
            height: data.height,
        },
        listItem: {
            position: 'relative',
            width: Math.ceil(data.width * 0.94),
            height: data.height / ratio,
            flexDirection: 'row',
            alignItems: 'center',
            marginTop: 10,
        },
        rankBg: {
            position: 'absolute',
            top: 0,
            left: 0,
            width: Math.ceil(data.width * 0.94),
            height: data.height / ratio,
        },
        rankAvatarBg: {
            position: 'absolute',
            top: (data.height / ratio) * 0.1,
            left: data.width * 0.08,
            width: (data.height / ratio) * 0.8,
            height: (data.height / ratio) * 0.8,
        },
        rankAvatar: {
            borderRadius: data.width * 0.05,
            marginLeft: data.width * 0.08 + (data.height / ratio) * 0.1,
            width: (data.height / ratio) * 0.7,
            height: (data.height / ratio) * 0.7,
        },
        rankNameView: {
            position: 'relative',
            marginLeft: data.width * 0.06,
            width: data.width * 0.35,
            height: data.height / ratio,
        },
        rankNameBg: {
            position: 'absolute',
            top: (data.height / ratio) * 0.14,
            left: 0,
            width: data.width * 0.35,
            height: (data.height / ratio) * 0.4,
        },
        rankName: {
            position: 'absolute',
            //top: (data.height / ratio) * 0.14,
            bottom: (data.height / ratio) * 0.3,
            left: 0,
            width: data.width * 0.35,
            height: (data.height / ratio) * 0.4,
            textAlign: 'left',
            lineHeight: (data.height / ratio) * 0.4,
            fontSize: data.width * 0.043,
            textOverflow: 'ellipsis',
            color: '#000',
        },
        rankNum: {
            position: 'absolute',
            bottom: (data.height / ratio) * 0.3,
            left: data.width * 0.035,
            width: data.width * 0.15,
            height: (data.height / ratio) * 0.3,
            lineHeight: (data.height / ratio) * 0.3,
            fontSize: data.width * 0.042,
            color: '#000',
            verticalAlign: 'middle',
        },
        rankScoreTip: {
            position: 'absolute',
            bottom: (data.height / ratio) * 0.3,
            left: data.width * 0.55,
            width: data.width * 0.15,
            height: (data.height / ratio) * 0.3,
            lineHeight: (data.height / ratio) * 0.3,
            fontSize: data.width * 0.042,
            color: '#000',
            textAlign: 'center',
        },
        rankScoreVal: {
            position: 'absolute',
            bottom: (data.height / ratio) * 0.3,
            left: data.width * 0.43,
            width: data.width * 0.15,
            height: (data.height / ratio) * 0.3,
            lineHeight: (data.height / ratio) * 0.3,
            fontSize: data.width * 0.042,
            color: '#000',
            textAlign: 'right',
        },
        shareNameView: {
            position: 'relative',
            marginLeft: data.width * 0.06,
            width: data.width * 0.35,
            height: (data.height / ratio) * 0.4,
        },
        shareNameBg: {
            position: 'absolute',
            top: 0,
            left: 0,
            width: data.width * 0.35,
            height: (data.height / ratio) * 0.4,
        },
        shareName: {
            position: 'absolute',
            top: 0,
            left: 0,
            width: data.width * 0.35,
            height: (data.height / ratio) * 0.4,
            textAlign: 'center',
            lineHeight: (data.height / ratio) * 0.4,
            fontSize: data.width * 0.043,
            textOverflow: 'ellipsis',
            color: '#fff',
        },
        shareToBtn: {
            position: 'relative',
            marginLeft: data.width * 0.08,
            width: data.width * 0.21,
            height: data.height * 0.16,
        },
        shareBtnBg: {
            position: 'absolute',
            right: 0,
            top: data.height * 0.16 * 0.25,
            width: data.width * 0.21,
            height: data.height * 0.16 * 0.5,
        },
        shareText: {
            position: 'absolute',
            right: 0,
            top: data.height * 0.16 * 0.25,
            width: data.width * 0.21,
            height: data.height * 0.16 * 0.5,
            lineHeight: data.height * 0.16 * 0.5,
            textAlign: 'center',
            fontSize: data.width * 0.043,
            color: '#fff',
        },
    };
}
