import { useState } from 'react';
import { QRCodeSVG } from 'qrcode.react';
import { Copy, Check, ExternalLink } from 'lucide-react';
import Modal from '../../components/ui/Modal';
import Button from '../../components/ui/Button';
import type { ShortLinkResponse } from './CreateLinkForm';

interface LinkResultPanelProps {
  isOpen: boolean;
  onClose: () => void;
  link: ShortLinkResponse;
}

const LinkResultPanel = ({ isOpen, onClose, link }: LinkResultPanelProps) => {
  const [copied, setCopied] = useState(false);

  const handleCopy = async () => {
    try {
      await navigator.clipboard.writeText(link.shortUrl);
      setCopied(true);
      setTimeout(() => setCopied(false), 2000);
    } catch (err) {
      console.error('Failed to copy text: ', err);
    }
  };

  return (
    <Modal isOpen={isOpen} onClose={onClose}>
      <div className="flex flex-col items-center text-center space-y-8">
        
        {/* Animated Success Icon & Title */}
        <div className="space-y-4">
          <div className="mx-auto w-16 h-16 bg-gradient-to-tr from-indigo-500 to-cyan-400 rounded-full flex items-center justify-center shadow-[0_0_30px_rgba(99,102,241,0.4)]">
            <Check size={32} className="text-white" strokeWidth={3} />
          </div>
          <h2 className="text-3xl font-bold bg-clip-text text-transparent bg-gradient-to-r from-white to-slate-400">
            Link is Ready!
          </h2>
        </div>
        
        {/* QR Code Container with Glow */}
        <div className="relative group">
          <div className="absolute -inset-1 bg-gradient-to-r from-indigo-500 to-cyan-400 rounded-2xl blur opacity-25 group-hover:opacity-40 transition duration-1000 group-hover:duration-200"></div>
          <div className="relative bg-white p-5 rounded-2xl shadow-xl">
            <QRCodeSVG 
              value={link.shortUrl} 
              size={180}
              level="H"
              includeMargin={false}
              className="rounded-lg"
            />
          </div>
        </div>

        {/* Link Display Box */}
        <div className="w-full space-y-3">
          <div className="flex items-center gap-2 p-2 pl-4 bg-slate-900/90 border border-slate-700/80 rounded-xl shadow-inner backdrop-blur-sm">
            <span className="flex-1 font-mono text-indigo-300 text-lg truncate text-left select-all">
              {link.shortUrl}
            </span>
            <div className="flex items-center gap-1">
              <button
                onClick={handleCopy}
                className="p-2.5 bg-indigo-600/10 text-indigo-400 hover:bg-indigo-600 hover:text-white rounded-lg transition-all duration-200"
                title="Copy to clipboard"
              >
                {copied ? <Check size={20} /> : <Copy size={20} />}
              </button>
              <a 
                href={link.shortUrl} 
                target="_blank" 
                rel="noopener noreferrer"
                className="p-2.5 text-slate-400 hover:text-cyan-400 bg-slate-800/50 hover:bg-slate-800 rounded-lg transition-all duration-200"
                title="Open link"
              >
                <ExternalLink size={20} />
              </a>
            </div>
          </div>
        </div>

        <Button onClick={onClose} variant="secondary" className="w-full py-4 text-lg font-medium shadow-lg hover:shadow-indigo-500/10">
          Done
        </Button>
      </div>
    </Modal>
  );
};

export default LinkResultPanel;
